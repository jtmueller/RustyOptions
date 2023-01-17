using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions.Async;

/// <summary>
/// Extension methods for async operations involving <see cref="Result{T, TErr}"/>.
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        if (self.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(self.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        if (self.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(self.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, T2> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = mapper(value);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, T2> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = mapper(value);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a result by applying a function to a contained <c>Ok</c> value, leaving an <c>Err</c> value untouched.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type contained by <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type contained by the return value.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result to map.</param>
    /// <param name="mapper">The function that converts a contained <c>Ok</c> value to <typeparamref name="T2"/>.</param>
    /// <returns>A result containing the mapped value, or <c>Err</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Result<T2, TErr>> MapAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var res = await self.ConfigureAwait(false);
        if (res.IsOk(out var value))
        {
            var mapped = await mapper(value).ConfigureAwait(false);
            return new(mapped);
        }

        return Result.Err<T2, TErr>(res.UnwrapErr());
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, T2> mapper, Func<TErr, T2> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, T2> mapper, Func<TErr, T2> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, ValueTask<T2>> mapper, Func<TErr, ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return self.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, Task<T2>> mapper, Func<TErr, Task<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return await self.Match(mapper, defaultFactory).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, ValueTask<T2>> mapper, Func<TErr, ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(mapper, defaultFactory).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, Task<T2>> mapper, Func<TErr, Task<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(mapper, defaultFactory).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, Task<T2>> mapper, Func<TErr, Task<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(mapper, defaultFactory).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously maps a <c>Result</c> by applying fallback function <paramref name="defaultFactory"/> to a
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
    public static async ValueTask<T2> MapOrElseAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, ValueTask<T2>> mapper, Func<TErr, ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(mapper, defaultFactory).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, ValueTask<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return self.Match(
            onOk: thenFunc,
            onErr: e => ValueTask.FromResult(Result.Err<T2, TErr>(e))
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, Task<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        return await self.Match(
            onOk: thenFunc,
            onErr: e => Task.FromResult(Result.Err<T2, TErr>(e))
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, Result<T2, TErr>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(
            onOk: thenFunc,
            onErr: Result.Err<T2, TErr>
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, Result<T2, TErr>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(
            onOk: thenFunc,
            onErr: Result.Err<T2, TErr>
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, ValueTask<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: thenFunc,
            onErr: e => ValueTask.FromResult(Result.Err<T2, TErr>(e))
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, ValueTask<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: thenFunc,
            onErr: e => ValueTask.FromResult(Result.Err<T2, TErr>(e))
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this ValueTask<Result<T1, TErr>> self, Func<T1, Task<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: thenFunc,
            onErr: e => Task.FromResult(Result.Err<T2, TErr>(e))
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise returns the <c>Err</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T1">The <c>Ok</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2">The <c>Ok</c> type returned by <paramref name="thenFunc"/>.</typeparam>
    /// <typeparam name="TErr">The <c>Err</c> type.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="thenFunc">The function to call with the <c>Ok</c> value, if any.</param>
    /// <returns>The result of calling <paramref name="thenFunc"/> if the result is <c>Ok</c>, otherwise the <c>Err</c> value of <paramref name="self"/>.</returns>
    public static async ValueTask<Result<T2, TErr>> AndThenAsync<T1, T2, TErr>(this Task<Result<T1, TErr>> self, Func<T1, Task<Result<T2, TErr>>> thenFunc)
        where T1 : notnull
        where TErr : notnull
        where T2 : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: thenFunc,
            onErr: e => Task.FromResult(Result.Err<T2, TErr>(e))
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, ValueTask<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        return self.Match(
            onOk: x => ValueTask.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, Task<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        return await self.Match(
            onOk: x => Task.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this ValueTask<Result<T, T1Err>> self, Func<T1Err, Result<T, T2Err>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: elseFunc
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this Task<Result<T, T1Err>> self, Func<T1Err, Result<T, T2Err>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return result.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: elseFunc
        );
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this ValueTask<Result<T, T1Err>> self, Func<T1Err, ValueTask<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: x => ValueTask.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this ValueTask<Result<T, T1Err>> self, Func<T1Err, Task<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: x => Task.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this Task<Result<T, T1Err>> self, Func<T1Err, ValueTask<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: x => ValueTask.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously calls <paramref name="elseFunc"/> if the result is <c>Err</c>, otherwise returns the <c>Ok</c> value of <paramref name="self"/>.
    /// </summary>
    /// <typeparam name="T">The <c>Ok</c> type of the result.</typeparam>
    /// <typeparam name="T1Err">The <c>Err</c> type of <paramref name="self"/>.</typeparam>
    /// <typeparam name="T2Err">The <c>Err</c> type returned by <paramref name="elseFunc"/>.</typeparam>
    /// <param name="self">The result.</param>
    /// <param name="elseFunc">The function to call with the <c>Err</c> value, if any.</param>
    /// <returns>The <c>Ok</c> value of the result, or the result of passing the <c>Err</c> value to <paramref name="elseFunc"/>.</returns>
    public static async ValueTask<Result<T, T2Err>> OrElseAsync<T, T1Err, T2Err>(this Task<Result<T, T1Err>> self, Func<T1Err, Task<Result<T, T2Err>>> elseFunc)
        where T : notnull
        where T1Err : notnull
        where T2Err : notnull
    {
        var result = await self.ConfigureAwait(false);
        return await result.Match(
            onOk: x => Task.FromResult(Result.Ok<T, T2Err>(x)),
            onErr: elseFunc
        ).ConfigureAwait(false);
    }
}
 