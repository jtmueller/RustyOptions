using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for the <see cref="Result{T, TErr}"/> type.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T2, TErr> Map<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return self.Match(
            onOk: x => new(mapper(x)),
            onErr: Result.Err<T2, TErr>
        );
    }

    /// <summary>
    /// Maps a result by applying a function to a contained <c>Err</c> value, leaving an <c>Ok</c> value untouched.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type contained by the return value.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="errMapper">The function that converts a contained <typeparamref name="T1Err"/> value to <typeparamref name="T2Err"/>.</param>
    /// <returns>A result containing the mapped error, or <c>Ok</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="errMapper"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, T2Err> MapErr<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, T2Err> errMapper)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        return self.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: e => new(errMapper(e))
        );
    }

    /// <summary>
    /// Returns the provided default (if <c>Err</c>), or applies a function to the contained value (if <c>Ok</c>).
    /// <para>
    /// Arguments passed to <see cref="MapOr{T1, T2, TErr}(Result{T1, TErr}, Func{T1, T2}, T2)"/> are eagerly evaluated;
    /// if you are passing the result of a function call, it is recommended to use
    /// <see cref="MapOrElse{T1, T2, TErr}(Result{T1, TErr}, Func{T1, T2}, Func{TErr, T2})"/>, which is lazily evaluated.
    /// </para>
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <param name="defaultValue">The default value to return if the result is <c>Err</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T2 MapOr<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper, T2 defaultValue)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);
        return self.IsOk(out var value) ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
    /// contained <c>Err</c> value, or function <paramref name="mapper"/> to a contained <c>Ok</c> value.
    /// <para>This function can be used to unpack a successful result while handling an error.</para>
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <param name="defaultFactory">The function that converts a contained <c>Err</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>The mapped value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T2 MapOrElse<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper, Func<TErr, T2> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return self.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Returns the contained <c>Ok</c> value, or a provided default.
    /// <para>
    /// Arguments passed to <see cref="UnwrapOr{T, TErr}(Result{T, TErr}, T)"/> are eagerly evaluated;
    /// if you are passing the result of a function call, it is recommended to use
    /// <see cref="Result{T, TErr}.UnwrapOrElse(Func{TErr, T})"/>, which is lazily evaluated.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to unwrap.</param>
    /// <param name="defaultValue">The default value to return if the result is <c>Err</c>.</param>
    /// <returns>The contained <c>Ok</c> value, or the provided default.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T UnwrapOr<T, TErr>(this Result<T, TErr> self, T defaultValue)
        where T : notnull
        where TErr : notnull
    {
        return self.IsOk(out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Returns <paramref name="other"/> if <paramref name="self"/> is <c>Ok</c>, otherwise
    /// returns the <c>Err</c> value of <paramref name="self"/>.
    /// <para>
    /// Arguments passed to and are eagerly evaluated; if you are passing the result of a function call,
    /// it is recommended to use <see cref="AndThen{T1, T2, TErr}(Result{T1, TErr}, Func{T1, Result{T2, TErr}})"/>, which is lazily evaluated.
    /// </para>
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type of <paramref name="other"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="other">The other result.</param>
    /// <returns>
    /// <paramref name="other"/> if <paramref name="self"/> is <c>Ok</c>, otherwise
    /// returns the <c>Err</c> value of <paramref name="self"/>.
    /// </returns>
    public static Result<T2, TErr> And<T1, T2, TErr>(this Result<T1, TErr> self, Result<T2, TErr> other)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var selfOk = !self.IsErr(out var selfErr);
        return selfOk ? other : Result.Err<T2, TErr>(selfErr!);
    }

    /// <summary>
    /// Calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type of <paramref name="other"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static Result<T2, TErr> AndThen<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, Result<T2, TErr>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return self.Match(
            onOk: thenFunc,
            onErr: Result.Err<T2, TErr>
        );
    }

    public static Result<T, T2Err> Or<T, T1Err, T2Err>(this Result<T, T1Err> self, Result<T, T2Err> other)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        return self.IsOk(out var value) ? Result.Ok<T, T2Err>(value) : other;
    }

    public static Result<T, T2Err> OrElse<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, Result<T, T2Err>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        return self.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: elseFunc
        );
    }

    public static Result<T, TErr> Flatten<T, TErr>(this Result<Result<T, TErr>, TErr> self)
        where T : notnull
        where TErr : notnull
    {
        return self.Match(
            onOk: x => x,
            onErr: Result.Err<T, TErr>
        );
    }
}
