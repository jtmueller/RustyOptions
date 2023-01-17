using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// This class contains static methods for creation an <see cref="Option{T}"/>.
/// </summary>
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not concerned with Visual Basic.")]
public static class Option
{
    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) where T : notnull => new(value);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> if it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value) where T : class => new(value!);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> is it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Create<T>(T? value)
        where T : struct
    {
        return value.HasValue ? new(value.GetValueOrDefault()) : default;
    }

    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() where T : notnull => default;

    /// <summary>
    /// Calls the provided <see cref="TryGet{T}"/> delegate
    /// and wraps the result as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to get.</typeparam>
    /// <param name="tryGet">The <see cref="TryGet{T}"/> function to call.</param>
    /// <returns>A function that will call the <paramref name="tryGet"/> function and wrap the results as an <see cref="Option{T}"/>.</returns>
    public static Option<T> Try<T>(TryGet<T> tryGet)
        where T : notnull
    {
        ThrowIfNull(tryGet);

        try
        {
            return tryGet(out var value) ? new(value!) : default;
        }
        catch (InvalidOperationException)
        {
            // JsonElement's TryGetXXX methods will throw an InvalidOperationException instead of
            // returning false if the underlying node type does not match the requested
            // data type (calling TryGetInt32 on a string node).
            return default;
        }
    }

    /// <summary>
    /// Generates a function that calls the provided <see cref="TryGetValue{TKey,TValue}"/> delegate
    /// and wraps the result as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type to get.</typeparam>
    /// <param name="tryGetValue">The <see cref="TryGetValue{TKey,TValue}"/> function to call.</param>
    /// <returns>A function that will call the <paramref name="tryGetValue"/> function and wrap the results as an <see cref="Option{T}"/>.</returns>
    public static Func<TKey, Option<TValue>> Bind<TKey, TValue>(TryGetValue<TKey, TValue> tryGetValue)
        where TValue : notnull
        where TKey : notnull
    {
        ThrowIfNull(tryGetValue);

        return (TKey key) => tryGetValue(key, out var value) ? new(value!) : default;
    }

#if NET7_0_OR_GREATER

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParsable{T}"/>.
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
    /// Parses a string into any type that supports <see cref="IParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(string s) where T : IParsable<T>
        => Parse<T>(s, provider: null);

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParsable{T}"/>.
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
    /// Parses a char span into any type that supports <see cref="ISpanParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Parse<T>(ReadOnlySpan<char> s) where T : ISpanParsable<T>
        => Parse<T>(s, provider: null);

#endif

    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <typeparam name="T">The enum type to parse into.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="ignoreCase">Whether to ignore case while parsing. Defaults to <c>true</c>.</param>
    /// <returns>An <see cref="Option{T}"/> containing the parsed enum, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ParseEnum<T>(string value, bool ignoreCase = true)
        where T : struct, Enum
    {
        return Enum.TryParse<T>(value, ignoreCase, out var parsed)
            ? new(parsed) : default;
    }

    /// <summary>
    /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <typeparam name="T">The enum type to parse into.</typeparam>
    /// <param name="value">The string to parse.</param>
    /// <param name="ignoreCase">Whether to ignore case while parsing. Defaults to <c>true</c>.</param>
    /// <returns>An <see cref="Option{T}"/> containing the parsed enum, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> ParseEnum<T>(ReadOnlySpan<char> value, bool ignoreCase = true)
        where T : struct, Enum
    {
        return Enum.TryParse<T>(value, ignoreCase, out var parsed)
            ? new(parsed) : default;
    }
}

/// <summary>
/// Delegate that represents a fallible attempt to get a value of a given type.
/// </summary>
/// <typeparam name="T">The type of value to get.</typeparam>
/// <param name="value">The value retrieved, if any.</param>
/// <returns><c>true</c> if the value was retrieved, otherwise <c>false</c>.</returns>
public delegate bool TryGet<T>([MaybeNullWhen(false)] out T? value);

/// <summary>
/// Delegate that represents a fallible attempt to get a value associated with a given key.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of value to get.</typeparam>
/// <param name="key">The key to retrieve the corresponding value for.</param>
/// <param name="value">The value retrieved, if any.</param>
/// <returns><c>true</c> if the value was retrieved, otherwise <c>false</c>.</returns>
public delegate bool TryGetValue<TKey, TValue>(TKey key, [MaybeNullWhen(false)] out TValue? value);

