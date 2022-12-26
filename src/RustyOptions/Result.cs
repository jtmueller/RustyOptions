﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.ArgumentNullException;

namespace RustyOptions;

// TODO: A Contains(T) method? Match that returns void?

/// <summary>
/// <see cref="Result{T, TErr}"/> is used to return the result of an operation that might fail, without
/// throwing an exception. Either <see cref="IsOk"/> will return <c>true</c> and the contained result value,
/// or else <see cref="IsErr"/> will return <c>true</c> and the contained error value.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
/// <typeparam name="TErr">The type of the error value.</typeparam>
[Serializable]
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

    /// <summary>
    /// Returns the result of executing the <paramref name="onOk"/>
    /// or <paramref name="onErr"/> functions, depending on the state 
    /// of the <see cref="Result{T, TErr}"/>.
    /// </summary>
    /// <typeparam name="T2">The return type of the given functions.</typeparam>
    /// <param name="onOk">The function to pass the value to, if the result is <c>Ok</c>.</param>
    /// <param name="onErr">The function to pass the error value to, if the result is <c>Err</c>.</param>
    /// <returns>The value returned by the called function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if either <paramref name="onOk"/> or <paramref name="onErr"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T2 Match<T2>(Func<T, T2> onOk, Func<TErr, T2> onErr)
    {
        ThrowIfNull(onOk);
        ThrowIfNull(onErr);
        return _isOk ? onOk(_value) : onErr(_err);
    }

    /// <summary>
    /// Returns the contained value if the result is <c>Ok</c>. Otherwise,
    /// throws an <see cref="InvalidOperationException"/>.
    /// <para>
    /// Note: if the <c>Err</c> is <see cref="Exception"/> it will be contained as an inner exception.
    /// Otherwise, the <c>Err</c> value will be converted to a string and included in the exception message.
    /// </para>
    /// </summary>
    /// <returns>The value inside the result, if the result is <c>Ok</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is in the error state.</exception>
    public T Unwrap()
    {
        if (_isOk)
            return _value;

        if (_err is Exception ex)
            throw new InvalidOperationException("Could not unwrap a Result in the Err state.", ex);

        throw new InvalidOperationException($"Could not unwrap a Result in the Err state: {_err}");
    }

    /// <summary>
    /// Returns the contained value if the result is <c>Ok</c>. Otherwise,
    /// throws an <see cref="InvalidOperationException"/> with the given message.
    /// <para>
    /// Note: if the <c>Err</c> is <see cref="Exception"/> it will be contained as an inner exception.
    /// Otherwise, the <c>Err</c> value will be converted to a string and included in the exception message.
    /// </para>
    /// </summary>
    /// <returns>The value inside the result, if the result is <c>Ok</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the result is in the error state.</exception>
    public T Expect(string message)
    {
        if (_isOk)
            return _value;

        if (_err is Exception ex)
            throw new InvalidOperationException(message, ex);

        throw new InvalidOperationException($"{message} - {_err}");
    }

    /// <summary>
    /// Converts the result into a <see cref="ReadOnlySpan{T}"/> that contains either zero or one
    /// items depending on whether the result is <c>Err</c> or <c>Ok</c>.
    /// </summary>
    /// <returns>A span containing the result's value, or an empty span. Error values are omitted.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan()
    {
        return _isOk
            ? MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in _value), 1)
            : ReadOnlySpan<T>.Empty;
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable{T}"/> containing either zero or one value,
    /// depending on whether the result is <c>Err</c> or <c>Ok</c>.
    /// </summary>
    /// <returns>An enumerable containing the result's value, or an empty enumerable. Error values are omitted.</returns>
    public IEnumerable<T> AsEnumerable()
    {
        if (_isOk)
        {
            yield return _value;
        }
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Result<T, TErr> other)
    {
        return (_isOk, other._isOk) switch
        {
            (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
            (false, false) => EqualityComparer<TErr>.Default.Equals(_err, other._err),
            _ => false
        };
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="obj"/> parameter;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Result<T, TErr> other && Equals(other);

    /// <summary>
    /// Retrieves the hash code of the object contained by the <see cref="Result{T, TErr}"/>, if any.
    /// </summary>
    /// <returns>
    /// The hash code of the object returned by the <see cref="IsOk(out T)"/> method, or <see cref="IsErr(out TErr)"/>,
    /// whichever method returns <c>true</c>.
    /// </returns>
    public override int GetHashCode()
        => _isOk ? _value.GetHashCode() : _err.GetHashCode();

    public override string ToString()
    {
        return _isOk ? $"Ok({_value})" : $"Err({_err})";
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
            if (_value is ISpanFormattable spanFormattable)
            {
                if ("Ok(".TryCopyTo(destination) &&
                    spanFormattable.TryFormat(destination[3..], out int valWritten, format, provider))
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
                string output = this.ToString(format.IsEmpty ? null : format.ToString(), provider);

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

    public static bool operator ==(Result<T, TErr> left, Result<T, TErr> right)
        => left.Equals(right);

    public static bool operator !=(Result<T, TErr> left, Result<T, TErr> right)
        => !left.Equals(right);

    public static bool operator >(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) > 0;

    public static bool operator <(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) < 0;

    public static bool operator >=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) >= 0;

    public static bool operator <=(Result<T, TErr> left, Result<T, TErr> right)
        => left.CompareTo(right) <= 0;
}

/// <summary>
/// <see cref="Result{T, TErr}"/> is used to return the result of an operation that might fail, without
/// throwing an exception.
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Ok</c> state, containing
    /// the given value.
    /// </summary>
    /// <typeparam name="T">The type of value the result contains.</typeparam>
    /// <typeparam name="TErr">The type of error the result may contain.</typeparam>
    /// <param name="value">The value to store in the result.</param>
    /// <returns>A result object containing the given value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Ok<T, TErr>(T value)
        where T : notnull
        where TErr : notnull
    {
        return new(value);
    }

    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Ok</c> state,
    /// with <c>string</c> as the error type.
    /// <para>This overload avoids explicit generic annotations when you want the error to be a simple message.</para>
    /// </summary>
    /// <typeparam name="T">The type of the value the result contains.</typeparam>
    /// <param name="value">The value to store in the result.</param>
    /// <returns>A result object containing the given value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, string> Ok<T>(T value)
        where T : notnull
    {
        return new(value);
    }

    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Ok</c> state,
    /// with <see cref="Exception"/> as the error type.
    /// <para>This overload avoids explicit generic annotations when converting try/catch code into a <c>Result</c>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the value the result contains.</typeparam>
    /// <param name="value">The value to store in the result.</param>
    /// <returns>A result object containing the given value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, Exception> OkExn<T>(T value)
        where T : notnull
    {
        return new(value);
    }

    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Err</c> state,
    /// containing the given error value.
    /// </summary>
    /// <typeparam name="T">The type of the value the result would contain if it were not in the <c>Err</c> state.</typeparam>
    /// <typeparam name="TErr">The type of the error the result contains.</typeparam>
    /// <param name="error">The error value to store in the result.</param>
    /// <returns>A result object containing the given error value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the error value is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TErr> Err<T, TErr>(TErr error)
        where T : notnull
        where TErr : notnull
    {
        return new(error);
    }

    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Err</c> state,
    /// containing the given error message.
    /// </summary>
    /// <typeparam name="T">The type of the value the result would contain if it were not in the <c>Err</c> state.</typeparam>
    /// <param name="errMsg">The error message to store in the result.</param>
    /// <returns>A result object containing the given error message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the error message is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, string> Err<T>(string errMsg)
        where T : notnull
    {
        return new(errMsg);
    }

    /// <summary>
    /// Creates a <see cref="Result{T, TErr}"/> in the <c>Err</c> state,
    /// containing the given exception.
    /// </summary>
    /// <typeparam name="T">The type of the value the result would contain if it were not in the <c>Err</c> state.</typeparam>
    /// <param name="ex">The exception to store in the result.</param>
    /// <returns>A result object containing the given exception.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the exception is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, Exception> ErrExn<T>(Exception ex)
        where T : notnull
    {
        return new(ex);
    }

    /// <summary>
    /// Attempts to call <paramref name="func"/>, wrapping the returned value in an <c>Ok</c> result.
    /// Any exceptions will be caught and returned in an <c>Err</c> result.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the given function.</typeparam>
    /// <param name="func">The function to attempt calling.</param>
    /// <returns>The return value of <paramref name="func"/> wrapped in <c>Ok</c>, or <c>Err</c> containing any exception that was thrown.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, Exception> Try<T>(Func<T> func)
        where T : notnull
    {
        try
        {
            return OkExn(func());
        }
        catch (Exception ex)
        {
            return ErrExn<T>(ex);
        }
    }
}
