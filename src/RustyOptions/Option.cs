namespace RustyOptions;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// <see cref="Option{T}"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not concerned with Visual Basic.")]
public readonly struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>, ISpanFormattable
    where T : notnull
{
    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    [SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Disagree in this case.")]
    public static Option<T> None
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => default;
    }

    private readonly bool _isSome;
    private readonly T _value;

    /// <summary>
    /// Creates an <see cref="Option{T}"/> containing the given value.
    /// <para>NOTE: Nulls are not allowed; a null value will result in a <c>None</c> option.</para>
    /// </summary>
    /// <param name="value">The value to wrap in an <see cref="Option{T}"/>.</param>
    public Option(T value)
    {
        _value = value;
        _isSome = value is not null;
    }

    /// <summary>
    /// Returns <c>true</c> if the option is <c>None</c>.
    /// </summary>
    public bool IsNone => !_isSome;

    /// <summary>
    /// Returns <c>true</c> if the option is <c>Some</c>, and returns the contained
    /// value through <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value contained in the option.</param>
    /// <returns><c>true</c> if the option is <c>Some</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isSome;
    }

    /// <summary>
    /// Converts the option into a <see cref="ReadOnlySpan{T}"/> that contains either zero or one
    /// items depending on whether the option is <c>Some</c> or <c>None</c>.
    /// </summary>
    /// <returns>A span containing the option's value, or an empty span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isSome
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    /// <summary>
    /// Returns an enumerator that allows the option to be iterated with a <c>foreach</c> loop.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
        => AsSpan().GetEnumerator();

    /// <summary>
    /// The Deconstruct method is used by the C# destructuring syntax to get the component
    /// parts from this Option.
    /// <code>
    ///   var (isSome, value) = Option.Some(42);
    ///   // isSome is true, value is 42
    /// </code>
    /// </summary>
    /// <param name="isSome">Whether or not this option represents a <c>Some</c> value.</param>
    /// <param name="value">The value contained in this option, if any.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out bool isSome, out T? value)
    {
        isSome = _isSome;
        value = _value;
    }

    /// <inheritdoc />
    public bool Equals(Option<T> other)
    {
        if (_isSome != other._isSome)
            return false;

        if (!_isSome)
            return true;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Option<T> opt && Equals(opt);

    /// <inheritdoc />
    public override int GetHashCode()
        => _isSome ? _value.GetHashCode() : 0;

    /// <inheritdoc />
    public override string ToString()
    {
        return _isSome 
            ? string.Create(CultureInfo.InvariantCulture, $"Some({_value})") 
            : "None";
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return _isSome
            ? string.IsNullOrEmpty(format)
                ? string.Create(formatProvider, $"Some({_value})")
                : string.Format(formatProvider, "Some({0:" + format + "})", _value)
            : "None";
    }

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (_isSome)
        {
            if (_value is ISpanFormattable formatVal)
            {
                if ("Some(".TryCopyTo(destination) && formatVal.TryFormat(destination[5..], out var innerWritten, format, provider))
                {
                    destination = destination[(5 + innerWritten)..];
                    if (destination.Length >= 1)
                    {
                        destination[0] = ')';
                        charsWritten = innerWritten + 6;
                        return true;
                    }
                }
            }
            else
            {
                var output = format.IsEmpty
                    ? string.Create(provider, $"Some({_value})")
                    : string.Format(provider, $"Some({{0:{format}}})", _value);

                if (output.TryCopyTo(destination))
                {
                    charsWritten = output.Length;
                    return true;
                }
            }
        }
        else if ("None".TryCopyTo(destination))
        {
            charsWritten = 4;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    /// <inheritdoc/>
    public int CompareTo(Option<T> other)
    {
        return (_isSome, other._isSome) switch
        {
            (true, true) => Comparer<T>.Default.Compare(_value, other._value),
            (true, false) => 1,
            (false, true) => -1,
            (false, false) => 0
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Option<T> left, Option<T> right)
        => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Option<T> left, Option<T> right)
        => !left.Equals(right);

    /// <inheritdoc />
    public static bool operator >(Option<T> left, Option<T> right)
        => left.CompareTo(right) > 0;

    /// <inheritdoc />
    public static bool operator <(Option<T> left, Option<T> right)
        => left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator >=(Option<T> left, Option<T> right)
        => left.CompareTo(right) >= 0;

    /// <inheritdoc />
    public static bool operator <=(Option<T> left, Option<T> right)
        => left.CompareTo(right) <= 0;
}

/// <summary>
/// <see cref="Option"/> represents an optional value: every <see cref="Option{T}"/> is either <c>Some</c> and contains a value, or <c>None</c>, and does not. 
/// </summary>
/// <typeparam name="T">The type the opton might contain.</typeparam>
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not concerned with Visual Basic.")]
public static class Option
{
    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) where T : notnull
        => new(value);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> if it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value) where T : class
        => new(value!);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> is it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value)
        where T : struct
    {
        return value.HasValue
            ? new(value.GetValueOrDefault())
            : Option<T>.None;
    }

    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() where T : notnull => default;

    /// <summary>
    /// Delegate that represents a fallible attempt to get a value of a given type.
    /// </summary>
    /// <typeparam name="T">The type of value to get.</typeparam>
    /// <param name="value">The value retrieved, if any.</param>
    /// <returns><c>true</c> if the value was retrieved, otherwise false.</returns>
    public delegate bool TryGet<T>([MaybeNullWhen(false)] out T? value);

    /// <summary>
    /// Generates a function that calls the provided <see cref="TryGet{T}"/> delegate
    /// and wraps the result as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to get.</typeparam>
    /// <param name="tryGet">The <see cref="TryGet{T}"/> function to call.</param>
    /// <returns>A function that will call the <paramref name="tryGet"/> function and wrap the results as an <see cref="Option{T}"/>.</returns>
    public static Option<T> Bind<T>(TryGet<T> tryGet)
        where T : notnull
    {
        try
        {
            return tryGet(out var value) ? new(value!) : default;
        }
        catch (Exception)
        {
            // JsonElement's TryGetXXX methods will throw an exception instead of
            // returning false if the underlying node type does not match the requested
            // data type (calling TryGetInt32 on a string node).
            return default;
        }
    }

    /// <summary>
    /// Delegate that represents a fallible attempt to get a value associated with a given key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of value to get.</typeparam>
    /// <param name="value">The value retrieved, if any.</param>
    /// <returns><c>true</c> if the value was retrieved, otherwise false.</returns>
    public delegate bool TryGetValue<TKey, TValue>(TKey key, [MaybeNullWhen(false)] out TValue? value);

    /// <summary>
    /// Generates a function that calls the provided <see cref="TryGetValue{TKey,TValue}"/> delegate
    /// and wraps the result as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type to get.</typeparam>
    /// <param name="tryGetValue">The <see cref="TryGetValue{TKey,TValue}"/> function to call.</param>
    /// <returns>A function that will call the <paramref name="tryGetValue"/> function and wrap the results as an <see cref="Option{T}"/>.</returns>
    public static Func<TKey, Option<TValue>> Bind<TKey, TValue>(TryGetValue<TKey, TValue> tryGetValue)
        where TValue : notnull where TKey : notnull
    {
        return (TKey key) => tryGetValue(key, out var value) ? new(value!) : default;
    }

#if NET7_0_OR_GREATER

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s, IFormatProvider? provider)
        where T : IParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? new(value) : default;
    }

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s) where T : IParsable<T>
        => Parse<T>(s, provider: null);

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : ISpanParsable<T>
    {
        return T.TryParse(s, provider, out var value)
            ? new(value) : default;
    }

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParseable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(ReadOnlySpan<char> s) where T : ISpanParsable<T>
        => Parse<T>(s, provider: null);

#endif
}
