using System.Runtime.CompilerServices;

namespace RustyOptions;

/// <summary>
/// This class contains static methods for creating a <see cref="Result{T, TErr}"/>.
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
    /// <exception cref="System.ArgumentNullException">Thrown if the value is null.</exception>
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
    /// <exception cref="System.ArgumentNullException">Thrown if the value is null.</exception>
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
    /// <exception cref="System.ArgumentNullException">Thrown if the value is null.</exception>
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
    /// <exception cref="System.ArgumentNullException">Thrown if the error value is null.</exception>
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
    /// <exception cref="System.ArgumentNullException">Thrown if the error message is null.</exception>
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
    /// <exception cref="System.ArgumentNullException">Thrown if the exception is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, Exception> Err<T>(Exception ex)
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
            return Err<T>(ex);
        }
    }
}

