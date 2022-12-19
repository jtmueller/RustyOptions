using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

public static class ResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T2, T2Err> Bind<T1, T1Err, T2, T2Err>(this Result<T1, T1Err> self,
                                                               Func<T1, Result<T2, T2Err>> okBinder,
                                                               Func<T1Err, Result<T2, T2Err>> errBinder)
        where T1 : notnull where T1Err : notnull where T2 : notnull where T2Err : notnull
    {
        return self.Match(okBinder, errBinder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T2, TErr> Map<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper)
        where T1 : notnull where TErr : notnull where T2 : notnull
    {
        return self.Match(
            onOk: x => new(mapper(x)),
            onErr: Result.Err<T2, TErr>
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, T2Err> MapErr<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, T2Err> errMapper)
        where T : notnull where T1Err : notnull where T2Err : notnull
    {
        return self.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: e => new(errMapper(e))
        );
    }

    public static T2 MapOr<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper, T2 defaultValue)
        where T1 : notnull where TErr : notnull where T2 : notnull
    {
        ThrowIfNull(mapper);
        return self.IsOk(out var value) ? mapper(value) : defaultValue;
    }

    public static T2 MapOrElse<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, T2> mapper, Func<T2> defaultFactory)
        where T1 : notnull where TErr : notnull where T2 : notnull
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
            ? Option.Some(err)
            : default;
    }

    public static Option<T> Ok<T, TErr>(this Result<T, TErr> self)
        where T : notnull where TErr : notnull
    {
        return self.IsOk(out var value)
            ? Option.Some(value)
            : default;
    }

    public static Option<Result<T, TErr>> Transpose<T, TErr>(this Result<Option<T>, TErr> self)
        where T : notnull where TErr : notnull
    {
        // Ok(None) will be mapped to None. Ok(Some(_)) and Err(_) will be mapped to Some(Ok(_)) and Some(Err(_)).

        var (isOk, opt, err) = self;

        if (isOk)
        {
            return opt.IsSome(out var value)
                ? Option.Some(Result.Ok<T, TErr>(value))
                : default;
        }

        return Option.Some(Result.Err<T, TErr>(err!));
    }

    public static Result<T2, TErr> And<T1, T2, TErr>(this Result<T1, TErr> self, Result<T2, TErr> other)
        where T1 : notnull where TErr : notnull where T2 : notnull
    {
        var selfOk = !self.IsErr(out var selfErr);
        var otherOk = other.IsOk(out _);

        if (selfOk == otherOk || selfOk)
            return other;

        return Result.Err<T2, TErr>(selfErr!);
    }

    public static Result<T2, TErr> AndThen<T1, T2, TErr>(this Result<T1, TErr> self, Func<T1, Result<T2, TErr>> thenFunc)
        where T1 : notnull where TErr : notnull where T2 : notnull
    {
        return self.Match(
            onOk: thenFunc,
            onErr: Result.Err<T2, TErr>
        );
    }

    public static Result<T, T2Err> Or<T, T1Err, T2Err>(this Result<T, T1Err> self, Result<T, T2Err> other)
        where T : notnull where T1Err : notnull where T2Err : notnull
    {
        if (self.IsOk(out var value))
            return Result.Ok<T, T2Err>(value);

        return other;
    }

    public static Result<T, T2Err> OrElse<T, T1Err, T2Err>(this Result<T, T1Err> self, Func<T1Err, Result<T, T2Err>> elseFunc)
        where T : notnull where T1Err : notnull where T2Err : notnull
    {
        return self.Match(
            onOk: Result.Ok<T, T2Err>,
            onErr: elseFunc
        );
    }
}
