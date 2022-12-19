using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

public static class ResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<U, UErr> Bind<T, TErr, U, UErr>(this Result<T, TErr> self,
                                                         Func<T, Result<U, UErr>> okBinder,
                                                         Func<TErr, Result<U, UErr>> errBinder)
        where T : notnull where TErr : notnull where U : notnull where UErr : notnull
    {
        return self.Match(okBinder, errBinder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<U, TErr> Map<T, TErr, U>(this Result<T, TErr> self, Func<T, U> mapper)
        where T : notnull where TErr : notnull where U : notnull
    {
        return self.Match(
            onOk: x => Result<U, TErr>.Ok(mapper(x)),
            onErr: Result<U, TErr>.Err
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, UErr> MapErr<T, TErr, UErr>(this Result<T, TErr> self, Func<TErr, UErr> errMapper)
        where T : notnull where TErr : notnull where UErr : notnull
    {
        return self.Match(
            onOk: Result<T, UErr>.Ok,
            onErr: e => Result<T, UErr>.Err(errMapper(e))
        );
    }

    public static U MapOr<T, TErr, U>(this Result<T, TErr> self, Func<T, U> mapper, U defaultValue)
        where T : notnull where TErr : notnull where U : notnull
    {
        ThrowIfNull(mapper);
        return self.IsOk(out var value) ? mapper(value) : defaultValue;
    }

    public static U MapOrElse<T, TErr, U>(this Result<T, TErr> self, Func<T, U> mapper, Func<U> defaultFactory)
        where T : notnull where TErr : notnull where U : notnull
    {
        ThrowIfNull(mapper);
        ThrowIfNull(defaultFactory);
        return self.IsOk(out var value) ? mapper(value) : defaultFactory();
    }

    public static T Expect<T, TErr>(this Result<T, TErr> self, string message)
        where T : notnull where TErr : notnull
    {
        var (isOk, value, err) = self;

        if (isOk)
            return value!;

        if (err is Exception ex)
            throw new InvalidOperationException(message, ex);

        throw new InvalidOperationException($"{message}: {err}");
    }

    public static T Unwrap<T, TErr>(this Result<T, TErr> self)
        where T : notnull where TErr : notnull
    {
        var (isOk, value, err) = self;

        if (isOk)
            return value!;

        if (err is Exception ex)
            throw new InvalidOperationException("Could not unwrap a Result in the Err state.", ex);

        throw new InvalidOperationException($"Could not unwrap a Result in the Err state: {err}");
    }

    public static T UnwrapOr<T, TErr>(this Result<T, TErr> self, T defaultValue)
        where T : notnull where TErr : notnull
    {
        return self.IsOk(out var value) ? value : defaultValue;
    }

    public static T UnwrapOrElse<T, TErr>(this Result<T, TErr> self, Func<T> elseFunction)
        where T : notnull where TErr : notnull
    {
        ThrowIfNull(elseFunction);
        return self.IsOk(out var value) ? value : elseFunction();
    }

    public static TErr ExpectErr<T, TErr>(this Result<T, TErr> self, string message)
        where T : notnull where TErr : notnull
    {
        if (self.IsErr(out var err))
            return err;

        throw new InvalidOperationException(message);
    }

    public static TErr UnwrapErr<T, TErr>(this Result<T, TErr> self)
        where T : notnull where TErr : notnull
    {
        if (self.IsErr(out var err))
            return err;

        throw new InvalidOperationException("Expected the result to be in the Err state, but it was Ok!");
    }

    public static Option<TErr> Err<T, TErr>(this Result<T, TErr> self)
        where T : notnull where TErr : notnull
    {
        return self.IsErr(out var err)
            ? Option<TErr>.Some(err)
            : Option<TErr>.None;
    }

    public static Option<T> Ok<T, TErr>(this Result<T, TErr> self)
        where T : notnull where TErr : notnull
    {
        return self.IsOk(out var value)
            ? Option<T>.Some(value)
            : Option<T>.None;
    }

    public static Option<Result<T, TErr>> Transpose<T, TErr>(this Result<Option<T>, TErr> self)
        where T : notnull where TErr : notnull
    {
        // Ok(None) will be mapped to None. Ok(Some(_)) and Err(_) will be mapped to Some(Ok(_)) and Some(Err(_)).

        var (isOk, opt, err) = self;

        if (isOk)
        {
            return opt.IsSome(out var value)
                ? Option.Some(Result<T, TErr>.Ok(value))
                : Option<Result<T, TErr>>.None;
        }

        return Option.Some(Result<T, TErr>.Err(err!));
    }

    public static Result<U, TErr> And<T, TErr, U>(this Result<T, TErr> self, Result<U, TErr> other)
        where T : notnull where TErr : notnull where U : notnull
    {
        var selfOk = !self.IsErr(out var selfErr);
        var otherOk = other.IsOk(out _);

        if (selfOk == otherOk || selfOk)
            return other;

        return Result<U, TErr>.Err(selfErr!);
    }

    public static Result<U, TErr> AndThen<T, TErr, U>(this Result<T, TErr> self, Func<T, Result<U, TErr>> thenFunc)
        where T : notnull where TErr : notnull where U : notnull
    {
        ThrowIfNull(thenFunc);
        var (isOk, val, err) = self;
        return isOk ? thenFunc(val!) : Result<U, TErr>.Err(err!);
    }

    public static Result<T, UErr> Or<T, TErr, UErr>(this Result<T, TErr> self, Result<T, UErr> other)
        where T : notnull where TErr : notnull where UErr : notnull
    {
        if (self.IsOk(out var value))
            return Result<T, UErr>.Ok(value);

        return other;
    }

    public static Result<T, UErr> OrElse<T, TErr, UErr>(this Result<T, TErr> self, Func<TErr, Result<T, UErr>> elseFunc)
        where T : notnull where TErr : notnull where UErr : notnull
    {
        var (isOk, val, err) = self;
        return isOk ? Result<T, UErr>.Ok(val!) : elseFunc(err!);
    }
}
