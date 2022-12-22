using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for the <see cref="Option{T}"/> type.
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// If the option has a value, passes that option to the mapper function and returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T1">The type of the option.</typeparam>
    /// <typeparam name="T2">The type returned by the mapper functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static Option<T2> Map<T1, T2>(this Option<T1> self, Func<T1, T2> mapper)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);

        return self.IsSome(out var value)
            ? Option.Some(mapper(value)) : default;
    }

    /// <summary>
    /// Returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to <see cref="MapOr"/> are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElse"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static T2 MapOr<T1, T2>(this Option<T1> self, Func<T1, T2> mapper, T2 defaultValue)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);
        return self.IsSome(out var value) ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T1">The type of the option's value.</typeparam>
    /// <typeparam name="T2">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static T2 MapOrElse<T1, T2>(this Option<T1> self, Func<T1, T2> mapper, Func<T2> defaultFactory)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(mapper);
        ThrowIfNull(defaultFactory);

        return self.IsSome(out var value) ? mapper(value) : defaultFactory();
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <param name="message">The message for the exception that gets thrown if the option has no value.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Expect<T>(this Option<T> self, string message)
        where T : notnull
    {
        return self.IsSome(out var value)
            ? value : throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or a provided default.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to bind.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    public static T UnwrapOr<T>(this Option<T> self, T defaultValue)
        where T : notnull
    {
        return self.IsSome(out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Returns the contained <c>Some</c> value or computes a default
    /// using the provided <paramref name="defaultFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <param name="defaultFactory">A function that returns a default value to use if the option is <c>None</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultFactory"/> is null.</exception>
    public static T UnwrapOrElse<T>(this Option<T> self, Func<T> defaultFactory)
        where T : notnull
    {
        ThrowIfNull(defaultFactory);
        return self.IsSome(out var value) ? value : defaultFactory();
    }

    /// <summary>
    /// Removes one level of nesting from nested options.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The nested option to flatten.</param>
    /// <returns>The given option with one level of nesting removed.</returns>
    public static Option<T> Flatten<T>(this Option<Option<T>> self)
        where T : notnull
    {
        return self.IsSome(out var nested) ? nested : default;
    }

    /// <summary>
    /// Returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="predicate"/>
    /// and returns <c>Some</c> if the predicated returns <c>true</c>, otherwise <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The option to check.</param>
    /// <param name="predicate">The function that determines if the value in the option is valid to return.</param>
    /// <returns><c>Some</c> if the option is <c>Some</c> and the predicate returns <c>true</c>, otherwise <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    public static Option<T> Filter<T>(this Option<T> self, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(predicate);

        return self.IsSome(out var value) && predicate(value) ? self : default;
    }

    /// <summary>
    /// Zips this <c>Option</c> with another <c>Option</c>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some((s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <returns>An option containing the values from both input options, if both have values. Otherwise, <c>None</c>.</returns>
    public static Option<(T1, T2)> Zip<T1, T2>(this Option<T1> self, Option<T2> other)
        where T1 : notnull
        where T2 : notnull
    {
        return self.IsSome(out var x) && other.IsSome(out var y)
            ? Option.Some((x, y)) : default;
    }

    /// <summary>
    /// Zips this <c>Option</c> and another <c>Option</c> with function <paramref name="zipper"/>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some(zipper(s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <typeparam name="T3">The type returned by the <paramref name="zipper"/> function.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <param name="zipper">A functon that combines values from the two options into a new type.</param>
    /// <returns>An option contianing the result of passing both values to the <paramref name="zipper"/> function, or <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="zipper"/> is null.</exception>
    public static Option<T3> ZipWith<T1, T2, T3>(this Option<T1> self, Option<T2> other, Func<T1, T2, T3> zipper)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        ThrowIfNull(zipper);

        return self.IsSome(out var x) && other.IsSome(out var y)
            ? Option.Some(zipper(x, y)) : default;
    }

    /// <summary>
    /// Returns <c>None</c> if <paramref name="self"/> is <c>None</c>, otherwise returns <paramref name="other"/>.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    public static Option<T2> And<T1, T2>(this Option<T1> self, Option<T2> other)
        where T1 : notnull
        where T2 : notnull
    {
        return self.IsNone ? default : other;
    }

    /// <summary>
    /// Returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type contained by the first option.</typeparam>
    /// <typeparam name="T2">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="thenFn">The function to call with the contained value, if there is a contained value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static Option<T2> AndThen<T1, T2>(this Option<T1> self, Func<T1, Option<T2>> thenFn)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(thenFn);
        return self.IsSome(out var value) ? thenFn(value) : default;
    }

    /// <summary>
    /// Returns <paramref name="self"/> if it contains a value, otherwise returns <paramref name="other"/>.
    /// <para>
    ///   Arguments passed to or are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use or_else, which is lazily evaluated.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The replacement option to use if the first option is <c>None</c>.</param>
    public static Option<T> Or<T>(this Option<T> self, Option<T> other)
        where T : notnull
    {
        return self.IsNone ? other : self;
    }

    /// <summary>
    /// Returns <paramref name="self"/> if it contains a value, otherwise calls <paramref name="elseFn"/> and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="elseFn">The function that creates the alternate value if the option is <c>None</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="elseFn"/> is null.</exception>
    public static Option<T> OrElse<T>(this Option<T> self, Func<Option<T>> elseFn)
        where T : notnull
    {
        ThrowIfNull(elseFn);
        return self.IsNone ? elseFn() : self;
    }

    /// <summary>
    /// Returns <c>Some</c> if exactly one of <paramref name="self"/>, <paramref name="other"/> is <c>Some</c>, otherwise returns <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the option.</typeparam>
    /// <param name="self">The option.</param>
    /// <param name="other">The other option.</param>
    public static Option<T> Xor<T>(this Option<T> self, Option<T> other)
        where T : notnull
    {
        if (self.IsNone)
            return other;

        if (other.IsNone)
            return self;

        return default;
    }

    /// <summary>
    /// Wraps the given value in an <see cref="Option{T}"/>.
    /// <para>NOTE: Null values will be returned as <c>None</c>, while non-null values will be returned as <c>Some</c>.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>The value wrapped in an <see cref="Option{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> AsOption<T>(this T? value)
        where T : class
    {
        return Option.Create(value);
    }

    /// <summary>
    /// Wraps the given value in an <see cref="Option{T}"/>.
    /// <para>NOTE: Null values will be returned as <c>None</c>, while non-null values will be returned as <c>Some</c>.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>The value wrapped in an <see cref="Option{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> AsOption<T>(this T? value)
        where T : struct
    {
        return Option.Create(value);
    }


    /// <summary>
    /// Wraps the given value in an <see cref="Option{T}"/>.
    /// <para>NOTE: Null values will be returned as <c>None</c>, while non-null values will be returned as <c>Some</c>.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>The value wrapped in an <see cref="Option{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(this T value)
        where T : notnull
    {
        return Option.Some(value);
    }
}
