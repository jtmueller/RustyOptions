using System.Runtime.CompilerServices;
using static System.ArgumentNullException;
using static RustyOptions.Option;

namespace RustyOptions.Async;

/// <summary>
/// Extension methods for async operations involving <see cref="Option{T}"/>.
/// </summary>
public static class OptionAsyncExtensions
{
    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this Option<T1> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        return self.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this Option<T1> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        return self.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, T2> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(mapper(value))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, T2> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(mapper(value))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, ValueTask<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// If the option has a value, passes that option to the mapper function and asynchronously returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<Option<T2>> MapAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, Task<T2>> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T2> MapOrElseAsync<T1, T2>(this Option<T1> self, Func<T1, ValueTask<T2>> mapper, Func<ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        return self.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this Option<T1> self, Func<T1, Task<T2>> mapper, Func<Task<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        return await self.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, T2> mapper, Func<T2> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        var opt = await self.ConfigureAwait(false);
        return opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, T2> mapper, Func<T2> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);

        var opt = await self.ConfigureAwait(false);
        return opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, ValueTask<T2>> mapper, Func<ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        var opt = await self.ConfigureAwait(false);
        return await opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, ValueTask<T2>> mapper, Func<ValueTask<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);

        var opt = await self.ConfigureAwait(false);
        return await opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, Task<T2>> mapper, Func<Task<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        var opt = await self.ConfigureAwait(false);
        return await opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static async ValueTask<T2> MapOrElseAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, Task<T2>> mapper, Func<Task<T2>> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);

        var opt = await self.ConfigureAwait(false);
        return await opt.Match(mapper, defaultFactory);
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static ValueTask<Option<T2>> AndThenAsync<T1, T2>(this Option<T1> self, Func<T1, ValueTask<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);

        return self.IsSome(out var value) ? thenFn(value) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this Option<T1> self, Func<T1, Task<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);

        return self.IsSome(out var value)
            ? await thenFn(value).ConfigureAwait(false) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, Option<T2>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value) ? thenFn(value) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, Option<T2>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value) ? thenFn(value) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, ValueTask<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value) ?
            await thenFn(value).ConfigureAwait(false) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, Task<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await thenFn(value).ConfigureAwait(false) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, ValueTask<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value) ?
            await thenFn(value).ConfigureAwait(false) : default;
    }

    /// <summary>
    /// Asynchronously returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static async ValueTask<Option<T2>> AndThenAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, Task<Option<T2>>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await thenFn(value).ConfigureAwait(false) : default;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static ValueTask<Option<T>> OrElseAsync<T>(this Option<T> self, Func<ValueTask<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);
        return self.IsNone ? elseFn() : ValueTask.FromResult(self);
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this Option<T> self, Func<Task<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);

        return self.IsNone
            ? await elseFn().ConfigureAwait(false)
            : self;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this ValueTask<Option<T>> self, Func<Option<T>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone ? elseFn() : opt;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this Task<Option<T>> self, Func<Option<T>> elseFn)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone ? elseFn() : opt;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this ValueTask<Option<T>> self, Func<ValueTask<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone
            ? await elseFn().ConfigureAwait(false)
            : opt;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this Task<Option<T>> self, Func<Task<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone
            ? await elseFn().ConfigureAwait(false)
            : opt;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this ValueTask<Option<T>> self, Func<Task<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone
            ? await elseFn().ConfigureAwait(false)
            : opt;
    }

    /// <summary>
    /// Asynchronously returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static async ValueTask<Option<T>> OrElseAsync<T>(this Task<Option<T>> self, Func<ValueTask<Option<T>>> elseFn)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(elseFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsNone
            ? await elseFn().ConfigureAwait(false)
            : opt;
    }
}
