using static System.ArgumentNullException;
using static RustyOptions.Option;

namespace RustyOptions;

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
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? Some(await mapper(value).ConfigureAwait(false))
            : default;
    }
}
