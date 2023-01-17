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
}
 