using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json;
using static System.ArgumentNullException;

namespace RustyOptions;

public static class OptionExtensions
{
    /// <summary>
    /// If the option has a value, passes that option to the mapper function and returns that value
    /// as a <c>Some</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <typeparam name="U">The type returned by the binder functions.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <returns>The mapped value as <c>Some</c>, or <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static Option<U> Map<T, U>(this Option<T> self, Func<T, U> mapper)
        where T : notnull where U : notnull
    {
        ThrowIfNull(mapper);

        return self.IsSome(out var value)
            ? Option<U>.Some(mapper(value))
            : Option<U>.None;
    }

    /// <summary>
    /// Returns the provided default result (if <c>None</c>), or applies a function to the contained value (if <c>Some</c>).
    /// <para>
    ///   Arguments passed to <see cref="MapOr"/> are eagerly evaluated; if you are passing the result of a function call,
    ///   it is recommended to use <see cref="MapOrElse"/>, which is lazily evaluated.</para>
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="U">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> is null.</exception>
    public static U MapOr<T, U>(this Option<T> self, Func<T, U> mapper, U defaultValue)
        where T : notnull where U : notnull
    {
        ThrowIfNull(mapper);
        return self.IsSome(out var value) ? mapper(value) : defaultValue;
    }

    /// <summary>
    /// Computes a default function result (if <c>None</c>), or applies a different function to the contained value (if <c>Some</c>).
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="U">The return type after mapping.</typeparam>
    /// <param name="self">The option to map.</param>
    /// <param name="mapper">The function that maps the value contained in the option.</param>
    /// <param name="defaultFactory">The function that lazily generates a default value, if required.</param>
    /// <param name="defaultValue">The default value to return if the option is <c>None</c>.</param>
    /// <returns>The mapped value, or the default value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="mapper"/> or <paramref name="defaultFactory"/> is null.</exception>
    public static U MapOrElse<T, U>(this Option<T> self, Func<T, U> mapper, Func<U> defaultFactory)
        where T : notnull where U : notnull
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
    /// Returns the contained <c>Some</c> value, or throws an <see cref="InvalidOperationException"/>
    /// with a generic message if the value is <c>None</c>.
    /// </summary>
    /// <typeparam name="T">The type of the option.</typeparam>
    /// <param name="self">The option to unwrap.</param>
    /// <returns>The value contained in the option.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the option does not contain a value.</exception>
    public static T Unwrap<T>(this Option<T> self)
        where T : notnull
    {
        return self.IsSome(out var value)
            ? value : throw new InvalidOperationException("The option was expected to contain a value, but did not.");
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
    /// Transforms the <see cref="Option{T}"/> into a <see cref="Result{T,TErr}"/>,
    /// mapping <c>Some</c> to <c>Ok</c> and <c>None</c> to <c>Err</c> using the provided
    /// <paramref name="error"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">The option to transform.</param>
    /// <param name="error">The error to use if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    public static Result<T, TErr> OkOr<T, TErr>(this Option<T> self, TErr error)
        where T : notnull where TErr : notnull
    {
        return self.IsSome(out var value)
            ? Result<T, TErr>.Ok(value)
            : Result<T, TErr>.Err(error);
    }

    /// <summary>
    /// Transforms the <see cref="Option{T}"/> into a <see cref="Result{T,TErr}"/>,
    /// mapping <c>Some</c> to <c>Ok</c> and <c>None</c> to <c>Err</c> using the provided
    /// <paramref name="errorFactory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the option's value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">The option to transform.</param>
    /// <param name="errorFactory">A function that creates an error object to be used if the option is <c>None</c>.</param>
    /// <returns>A <see cref="Result{T,TErr}"/> that contains either the option's value, or the provided error.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorFactory"/> is null.</exception>
    public static Result<T, TErr> OkOrElse<T, TErr>(this Option<T> self, Func<TErr> errorFactory)
        where T : notnull where TErr : notnull
    {
        ThrowIfNull(errorFactory);
        return self.IsSome(out var value)
            ? Result<T, TErr>.Ok(value)
            : Result<T, TErr>.Err(errorFactory());
    }

    /// <summary>
    /// Transposes an <c>Option</c> of a <c>Result</c> into a <c>Result</c> of an <c>Option</c>.
    /// <para>
    ///     <c>None</c> will be mapped to <c>Ok(None)</c>. 
    ///     <c>Some(Ok(_))</c> and <c>Some(Err(_))</c> will be mapped to <c>Ok(Some(_))</c> and <c>Err(_)</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">An option containing a result.</param>
    /// <returns>An equivalent result containing an option.</returns>
    public static Result<Option<T>, TErr> Transpose<T, TErr>(this Option<Result<T, TErr>> self)
        where T : notnull where TErr : notnull
    {
        if (self.IsSome(out var result))
        {
            return result.Match(
                onOk: val => Result<Option<T>, TErr>.Ok(Option<T>.Some(val)),
                onErr: Result<Option<T>, TErr>.Err
            );
        }

        return Result<Option<T>, TErr>.Ok(Option<T>.None);
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
        return self.IsSome(out var nested) ? nested : Option<T>.None;
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

        if (self.IsSome(out var value) && predicate(value))
        {
            return self;
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Zips this <c>Option</c> with another <c>Option</c>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some((s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <returns>An option containing the values from both input options, if both have values. Otherwise, <c>None</c>.</returns>
    public static Option<(T, U)> Zip<T, U>(this Option<T> self, Option<U> other)
        where T : notnull where U : notnull
    {
        if (self.IsSome(out var x) && other.IsSome(out var y))
        {
            return Option<(T, U)>.Some((x, y));
        }

        return Option<(T, U)>.None;
    }

    /// <summary>
    /// Zips this <c>Option</c> and another <c>Option</c> with function <paramref name="zipper"/>.
    /// <para>If this option is <c>Some(s)</c> and other is <c>Some(o)</c>, this method returns <c>Some(zipper(s, o))</c>. Otherwise, <c>None</c> is returned.</para>
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <typeparam name="V">The type returned by the <paramref name="zipper"/> function.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <param name="zipper">A functon that combines values from the two options into a new type.</param>
    /// <returns>An option contianing the result of passing both values to the <paramref name="zipper"/> function, or <c>None</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="zipper"/> is null.</exception>
    public static Option<V> ZipWith<T, U, V>(this Option<T> self, Option<U> other, Func<T, U, V> zipper)
        where T : notnull where U : notnull where V : notnull
    {
        ThrowIfNull(zipper);

        if (self.IsSome(out var x) && other.IsSome(out var y))
        {
            return Option<V>.Some(zipper(x, y));
        }

        return Option<V>.None;
    }

    /// <summary>
    /// Returns <c>None</c> if <paramref name="self"/> is <c>None</c>, otherwise returns <paramref name="other"/>.
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    public static Option<U> And<T, U>(this Option<T> self, Option<U> other)
        where T : notnull where U : notnull
    {
        return self.IsNone ? Option<U>.None : other;
    }

    /// <summary>
    /// Returns <c>None</c> if the option is <c>None</c>, otherwise calls <paramref name="thenFn"/> with the wrapped value and returns the result.
    /// </summary>
    /// <typeparam name="T">The type contained by the first option.</typeparam>
    /// <typeparam name="U">The type contained by the second option.</typeparam>
    /// <param name="self">The first option.</param>
    /// <param name="other">The second option.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="thenFn"/> is null.</exception>
    public static Option<U> AndThen<T, U>(this Option<T> self, Func<T, Option<U>> thenFn)
        where T : notnull where U : notnull
    {
        ThrowIfNull(thenFn);
        return self.IsSome(out var value) ? thenFn(value) : Option<U>.None;
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

        return Option<T>.None;
    }

    /// <summary>
    /// Wraps the given value in an <see cref="Option{T}"/>.
    /// <para>NOTE: Null values will be returned as <c>None</c>, while non-null values will be returned as <c>Some</c>.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns>The value wrapped in an <see cref="Option{T}"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(this T? value)
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
    public static Option<T> Some<T>(this T? value)
        where T : struct
    {
        return Option.Create(value);
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(key, out var value)
            ? Option<TValue>.Some(value)
            : Option<TValue>.None;
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(key, out var value)
            ? Option<TValue>.Some(value)
            : Option<TValue>.None;
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        // This overload is needed to disambiguate between IDictionary and IReadOnlyDictionary,
        // as Dictionary implements both.
        ThrowIfNull(self);

        return self.TryGetValue(key, out var value)
            ? Option<TValue>.Some(value)
            : Option<TValue>.None;
    }

    /// <summary>
    /// Attempts to get the underlying JSON value as the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns><c>Some(value)</c> if the underlying value could be successfully converted, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetOption<T>(this JsonValue self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue<T>(out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

    /// <summary>
    /// Attempts to get a property with the given name as the given data type. Does not support array properties - use GetPropOption for arrays.
    /// </summary>
    /// <typeparam name="T">The value type to convert to.</typeparam>
    /// <param name="self">The JsonObject to get the property from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>None</c> if the property was not present or could not be parsed into the indicated type. Otherwise, <c>Some(value)</c>.</returns>
    public static Option<T> GetPropValue<T>(this JsonObject self, string propertyName)
        where T : notnull
    {
        ThrowIfNull(self);
        if (self.TryGetPropertyValue(propertyName, out var node))
        {
            return node switch
            {
                JsonValue val => val.GetOption<T>(),
                JsonArray => Option<T>.None, // JsonArray isn't supported for this use
                JsonObject obj => obj.AsValue().GetOption<T>(),
                JsonNode n => n.AsValue().GetOption<T>(),
                _ => Option<T>.None
            };
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Gets the indicated propery node as an <see cref="Option{T}"/>.
    /// </summary>
    /// <param name="self">The JSON node to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>None</c> if the property was not present, otherwise <c>Some(node)</c></returns>
    public static Option<JsonNode> GetPropOption(this JsonObject self, string propertyName)
    {
        ThrowIfNull(self);
        if (self.TryGetPropertyValue(propertyName, out var node))
        {
            return Option.Some(node!);
        }

        return Option<JsonNode>.None;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, string propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option<JsonElement>.Some(value)
            : Option<JsonElement>.None;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, ReadOnlySpan<char> propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option<JsonElement>.Some(value)
            : Option<JsonElement>.None;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, ReadOnlySpan<byte> propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option<JsonElement>.Some(value)
            : Option<JsonElement>.None;
    }
}
