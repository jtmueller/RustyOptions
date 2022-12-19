using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// <see cref="Result{T, TErr}"/> is used to return the result of an operation that might fail, without
/// throwing an exception. Either <see cref="IsOk"/> will return <c>true</c> and the contained result value,
/// or else <see cref="IsErr"/> will return <c>true</c> and the contained error object.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
/// <typeparam name="TErr">The type of the error value.</typeparam>
public readonly struct Result<T, TErr> : IEquatable<Result<T, TErr>>, IComparable<Result<T, TErr>>, ISpanFormattable
    where T : notnull where TErr : notnull
{
    /// <summary>
    /// Initializes a <see cref="Result{T, TErr}"/> in the <c>Ok</c> state containing the given value.
    /// </summary>
    /// <param name="value">The value to contain.</param>
    /// <exception cref="ArgumentNullException">Thrown if the given value is null.</exception>
    public Result(T value)
    {
        ThrowIfNull(value);
        _value = value;
        _err = default!;
        _isOk = true;
    }

    /// <summary>
    /// Initializes a <see cref="Result{T, TErr}"/> in the <c>Err</c> state containing the given error value.
    /// </summary>
    /// <param name="error">The error value to store.</param>
    /// <exception cref="ArgumentNullException">Thrown if the given error is null.</exception>
    public Result(TErr error)
    {
        ThrowIfNull(error);
        _err = error;
        _value = default!;
        _isOk = false;
    }

    private readonly bool _isOk;
    private readonly T _value;
    private readonly TErr _err;

    /// <summary>
    /// Returns <c>true</c> if the result is in the <c>Ok</c> state, and <paramref name="value"/> will contain the return value.
    /// </summary>
    /// <param name="value">The returned value, if any.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isOk;
    }

    /// <summary>
    /// Returnd <c>true</c> if the result is in the <c>Err</c> state, and <paramref name="error"/> will contain the error value.
    /// </summary>
    /// <param name="error">The returned error value, if any.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErr([MaybeNullWhen(false)] out TErr error)
    {
        error = _err;
        return !_isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T2 Match<T2>(Func<T, T2> onOk, Func<TErr, T2> onErr)
    {
        ThrowIfNull(onOk);
        ThrowIfNull(onErr);
        return _isOk ? onOk(_value) : onErr(_err);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isOk
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReadOnlySpan<T>.Enumerator GetEnumerator()
    {
        return AsSpan().GetEnumerator();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return _isOk
            ? string.Create(CultureInfo.InvariantCulture, $"Ok({_value})")
            : string.Create(CultureInfo.InvariantCulture, $"Err({_err})");
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format))
        {
            return _isOk
                ? string.Create(formatProvider, $"Ok({_value})")
                : string.Create(formatProvider, $"Err({_err})");
        }

        return _isOk
            ? string.Format(formatProvider, "Ok({0:" + format + "})", _value)
            : string.Format(formatProvider, "Err({0:" + format + "})", _err);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (_isOk)
        {
            if (_value is ISpanFormattable formatVal)
            {
                if ("Ok(".TryCopyTo(destination) &&
                    formatVal.TryFormat(destination[3..], out int valWritten, format, provider))
                {
                    destination = destination[(3 + valWritten)..];
                    if (destination.Length >= 1)
                    {
                        destination[0] = ')';
                        charsWritten = valWritten + 4;
                        return true;
                    }
                }
            }
            else
            {
                string output = format.IsEmpty
                    ? string.Create(provider, $"Ok({_value})")
                    : string.Format(provider, $"Ok({{0:{format}}})", _value);

                if (output.TryCopyTo(destination))
                {
                    charsWritten = output.Length;
                    return true;
                }
            }
        }
        else
        {
            if (_err is ISpanFormattable formatErr)
            {
                if ("Err(".TryCopyTo(destination) &&
                    formatErr.TryFormat(destination[4..], out int errWritten, ReadOnlySpan<char>.Empty, provider))
                {
                    destination = destination[(4 + errWritten)..];
                    if (destination.Length >= 1)
                    {
                        destination[0] = ')';
                        charsWritten = errWritten + 5;
                        return true;
                    }
                }
            }
            else
            {
                string output = string.Create(provider, $"Err({_err})");

                if (output.TryCopyTo(destination))
                {
                    charsWritten = output.Length;
                    return true;
                }
            }
        }

        charsWritten = 0;
        return false;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out bool isOk, out T? value, out TErr? err)
    {
        isOk = _isOk;
        value = _value;
        err = _err;
    }

    /// <inheritdoc />
    public bool Equals(Result<T, TErr> other)
    {
        return (_isOk, other._isOk) switch
        {
            (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
            (false, false) => EqualityComparer<TErr>.Default.Equals(_err, other._err),
            _ => false
        };
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Result<T, TErr> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
        => _isOk ? _value.GetHashCode() : _err.GetHashCode();

    public int CompareTo(Result<T, TErr> other)
    {
        // Ok compares as less than any Err, while two Ok or two Err compare as their contained values would in T or E respectively.
        return (_isOk, other._isOk) switch
        {
            (true, true) => Comparer<T>.Default.Compare(_value, other._value),
            (true, false) => -1,
            (false, true) => 1,
            (false, false) => Comparer<TErr>.Default.Compare(_err, other._err)
        };
    }

    /// <inheritdoc />
    public static bool operator ==(Result<T, TErr> left, Result<T, TErr> right)
        => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Result<T, TErr> left, Result<T, TErr> right)
        => !left.Equals(right);

    /// <inheritdoc />
    public static bool operator >(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) > 0;

    /// <inheritdoc />
    public static bool operator <(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) < 0;

    /// <inheritdoc />
    public static bool operator >=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) >= 0;

    /// <inheritdoc />
    public static bool operator <=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) <= 0;
}

public static class Result
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Ok<T, TErr>(T value)
        where T : notnull where TErr : notnull
    {
        return new(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Err<T, TErr>(TErr error)
        where T : notnull where TErr : notnull
    {
        return new(error);
    }
}
