using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(Option{T1}, Func{T1, ValueTask{T2}}, Func{ValueTask{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static ValueTask<T2> MapOrAsync<T1, T2>(this Option<T1> self, Func<T1, ValueTask<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);
        return self.IsSome(out var value)
            ? mapper(value)
            : ValueTask.FromResult(defaultValue);
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(Option{T1}, Func{T1, Task{T2}}, Func{Task{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this Option<T1> self, Func<T1, Task<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);
        return self.IsSome(out var value)
            ? await mapper(value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(ValueTask{Option{T1}}, Func{T1, T2}, Func{T2})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, T2> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(Task{Option{T1}}, Func{T1, T2}, Func{T2})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, T2> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(ValueTask{Option{T1}}, Func{T1, ValueTask{T2}}, Func{ValueTask{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, ValueTask<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await mapper(value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(Task{Option{T1}}, Func{T1, ValueTask{T2}}, Func{ValueTask{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, ValueTask<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await mapper(value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(ValueTask{Option{T1}}, Func{T1, Task{T2}}, Func{Task{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this ValueTask<Option<T1>> self, Func<T1, Task<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await mapper(value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <summary>
    /// Asynchronously returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to this method are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElseAsync{T1, T2}(Task{Option{T1}}, Func{T1, Task{T2}}, Func{Task{T2}})"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static async ValueTask<T2> MapOrAsync<T1, T2>(this Task<Option<T1>> self, Func<T1, Task<T2>> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await mapper(value).ConfigureAwait(false)
            : defaultValue;
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
        ThrowIfNull(thenFn);

        var opt = await self.ConfigureAwait(false);
        return opt.IsSome(out var value)
            ? await thenFn(value).ConfigureAwait(false) : default;
    }
}
