using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods that transform Option to Result, or Result to Option.
/// </summary>
public static class OptionResultExtensions
{
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
        where T : notnull
        where TErr : notnull
    {
        return self.IsSome(out var value)
            ? new(value) : new(error);
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
        where T : notnull
        where TErr : notnull
    {
        ThrowIfNull(errorFactory);
        return self.IsSome(out var value)
            ? new(value) : new(errorFactory());
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
        where T : notnull
        where TErr : notnull
    {
        if (self.IsSome(out var result))
        {
            return result.Match(
                onOk: val => Result.Ok<Option<T>, TErr>(Option.Some(val)),
                onErr: Result.Err<Option<T>, TErr>
            );
        }

        return Result.Ok<Option<T>, TErr>(default);
    }

    /// <summary>
    /// Converts from the <c>Err</c> state of <see cref="Result{T, TErr}"/> to <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">The result to be converted.</param>
    /// <returns><c>Some(TErr)</c> if the result is <c>Err</c>, otherwise <c>None</c>.</returns>
    public static Option<TErr> Err<T, TErr>(this Result<T, TErr> self)
        where T : notnull
        where TErr : notnull
    {
        return self.IsErr(out var err)
            ? Option.Some(err)
            : default;
    }

    /// <summary>
    /// Converts from the <c>Ok</c> state of <see cref="Result{T, TErr}"/> to <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">The result to be converted.</param>
    /// <returns><c>Some(T)</c> if the result is <c>Ok</c>, otherwise <c>None</c>.</returns>
    public static Option<T> Ok<T, TErr>(this Result<T, TErr> self)
        where T : notnull
        where TErr : notnull
    {
        return self.IsOk(out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Transposes a <c>Result</c> of an <c>Option</c> into an <c>Option</c> of a <c>Result</c>.
    /// <para>
    ///     <c>Ok(None)</c> will be mapped to <c>None</c>. 
    ///     <c>Ok(Some(_))</c> and <c>Err(_)</c> will be mapped to <c>Some(Ok(_))</c> and <c>Some(Err(_))</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TErr">The type of the error.</typeparam>
    /// <param name="self">A result containing an option.</param>
    /// <returns>An equivalent option containing a result.</returns>
    public static Option<Result<T, TErr>> Transpose<T, TErr>(this Result<Option<T>, TErr> self)
        where T : notnull
        where TErr : notnull
    {
        return self.Match(
            onOk: x => x.IsSome(out var value) ? Option.Some(Result.Ok<T, TErr>(value)) : default,
            onErr: e => Option.Some(Result.Err<T, TErr>(e))
        );
    }
}

