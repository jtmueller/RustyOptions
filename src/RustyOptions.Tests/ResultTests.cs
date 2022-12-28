using System.Globalization;

namespace RustyOptions.Tests;

public sealed class ResultTests
{
    [Fact]
    public void CanPerformBasicOperationsStructClass()
    {
        var okInt = Result.Ok<int, string>(42);
        var errStr = Result.Err<int, string>("Whoops!");

        var okInt2 = Result.Ok<int, string>(42);
        var errStr2 = Result.Err<int, string>("Whoops!");

        Assert.True(okInt.IsOk(out var o1) && o1 == 42);
        Assert.True(errStr.IsErr(out var e1) && e1 == "Whoops!");
        Assert.False(okInt.IsErr(out _));
        Assert.False(errStr.IsOk(out _));

        Assert.True(okInt == okInt2);
        Assert.Equal(okInt, okInt2);
        Assert.True(okInt.Equals((object)okInt2));

        Assert.True(errStr == errStr2);
        Assert.Equal(errStr, errStr2);
        Assert.True(errStr.Equals((object)errStr2));
    }

    [Fact]
    public void CanPerformBasicOperationsClassStruct()
    {
        var okStr = Result.Ok<string, int>("Foo");
        var errInt = Result.Err<string, int>(-1);

        var okStr2 = Result.Ok<string, int>("Foo");
        var errInt2 = Result.Err<string, int>(-1);

        Assert.True(okStr.IsOk(out var o1) && o1 == "Foo");
        Assert.True(errInt.IsErr(out var e1) && e1 == -1);
        Assert.False(okStr.IsErr(out _));
        Assert.False(errInt.IsOk(out _));

        Assert.True(okStr == okStr2);
        Assert.Equal(okStr, okStr2);
        Assert.True(okStr.Equals(okStr2));

        Assert.True(errInt == errInt2);
        Assert.Equal(errInt, errInt2);
        Assert.True(errInt.Equals((object)errInt2));
    }

    [Fact]
    public void CanCreateWithStringErr()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        Assert.True(ok.IsOk(out var okVal) && okVal == 42);
        Assert.True(err.IsErr(out var errVal) && errVal == "oops");
    }

    [Fact]
    public void CanCreateWithExceptionErr()
    {
        var ok = Result.OkExn(42);
        var err = Result.ErrExn<int>(new InvalidOperationException("oops"));

        Assert.True(ok.IsOk(out var okVal) && okVal == 42);
        Assert.True(err.IsErr(out var ex) && ex.Message == "oops");
    }

    [Fact]
    public void CanMatch()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okResult = ok.Match(
            onOk: x => x * 2,
            onErr: _ => -1
        );

        var errResult = err.Match(
            onOk: x => x * 2,
            onErr: _ => -1
        );

        Assert.Equal(84, okResult);
        Assert.Equal(-1, errResult);
    }

    [Fact]
    public void CanUnwrap()
    {
        var ok = Result.Ok(42);
        var errStr = Result.Err<int>("oops");
        var errExn = Result.ErrExn<int>(new AggregateException("oops"));

        Assert.Equal(42, ok.Unwrap());

        var ex1 = Assert.Throws<InvalidOperationException>(() => errStr.Unwrap());
        Assert.EndsWith(": oops", ex1.Message, StringComparison.Ordinal);

        var ex2 = Assert.Throws<InvalidOperationException>(() => errExn.Unwrap());
        Assert.True(ex2.InnerException is AggregateException { Message: "oops" });
    }

    [Fact]
    public void CanExpect()
    {
        var ok = Result.Ok(42);
        var errStr = Result.Err<int>("oops");
        var errExn = Result.ErrExn<int>(new AggregateException("oops"));

        Assert.Equal(42, ok.Expect("No value found"));

        var ex1 = Assert.Throws<InvalidOperationException>(() => errStr.Expect("No value found"));
        Assert.Equal("No value found - oops", ex1.Message);

        var ex2 = Assert.Throws<InvalidOperationException>(() => errExn.Expect("No value found"));
        Assert.Equal("No value found", ex2.Message);
        Assert.True(ex2.InnerException is AggregateException { Message: "oops" });
    }

    [Fact]
    public void CanGetSpan()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okSpan = ok.AsSpan();
        var errSpan = err.AsSpan();

        Assert.False(okSpan.IsEmpty);
        Assert.True(errSpan.IsEmpty);

        Assert.Equal(1, okSpan.Length);
        Assert.Equal(0, errSpan.Length);

        Assert.Equal(42, okSpan[0]);
    }

    [Fact]
    public void CanEnumerate()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        int value = 0;
        foreach (var x in err.AsEnumerable())
        {
            value += x;
        }

        Assert.Equal(0, value);

        foreach (var x in ok.AsEnumerable())
        {
            value += x;
        }

        Assert.Equal(42, value);

        Assert.Equal(42, ok.AsEnumerable().FirstOrDefault());
        Assert.Equal(0, err.AsEnumerable().FirstOrDefault());
    }

    [Fact]
    public void CanTry()
    {
        int Throws() => Array.Empty<int>()[0];
        int DoesNotThrow() => new[] { 42 }[0];

        Assert.True(Result.Try(Throws).IsErr(out var ex) && ex is IndexOutOfRangeException);
        Assert.True(Result.Try(DoesNotThrow).IsOk(out var val) && val == 42);
    }

    [Fact]
    public void CanEquate()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");
        var sameOk = Result.Ok(42);
        var otherOk = Result.Ok(-1);

        Assert.Equal(ok, sameOk);
        Assert.NotEqual(ok, err);
        Assert.NotEqual(ok, otherOk);

        Assert.True(ok == sameOk);
        Assert.True(ok != err);
        Assert.True(ok != otherOk);

        Assert.True(ok.Equals((object)sameOk));
        Assert.False(ok.Equals((object)err));
        Assert.False(ok.Equals((object)otherOk));

        Assert.Equal(ok.GetHashCode(), sameOk.GetHashCode());
        Assert.NotEqual(ok.GetHashCode(), err.GetHashCode());
        Assert.NotEqual(ok.GetHashCode(), otherOk.GetHashCode());
    }

    [Fact]
    public void CanGetString()
    {
        var ok = Result.Ok(4200);
        var err = Result.Err<int>("oops");

        Assert.Equal("Ok(4200)", ok.ToString());
        Assert.Equal("Err(oops)", err.ToString());
        Assert.Equal("Ok(4,200.00)", ok.ToString("n2", CultureInfo.InvariantCulture));
        Assert.Equal("Err(oops)", err.ToString("n2", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void CanFormatToSpan()
    {
        var ok = Result.Ok(4200);
        var err = Result.Err<int>("oops");
        var okNotSpanFormattable = Result.Ok(new NotSpanFormattable { Value = 4200 });
        var okNotFormattable = Result.Ok(new NotFormattable { Value = 4200 });
        var errNotSpanFormattable = Result.Err<int, NotSpanFormattable>(new NotSpanFormattable { Value = -1 });
        var errNotFormattable = Result.Err<int, NotFormattable>(new NotFormattable { Value = -1 });

        Span<char> buffer = stackalloc char[255];

        Assert.True(ok.TryFormat(buffer, out int written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Ok(4200)"));

        Assert.True(err.TryFormat(buffer, out written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Err(oops)"));

        Assert.True(ok.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Ok(4,200.00)"));

        Assert.True(okNotSpanFormattable.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Ok(4,200.00)"));

        Assert.True(okNotFormattable.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Ok(4200)"));

        Assert.True(errNotSpanFormattable.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Err(-1.00)"));

        Assert.True(errNotFormattable.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Err(-1)"));
    }

    [Fact]
    public void CanCompare()
    {
        var a = Result.Ok(1);
        var b = Result.Ok(2);
        var c = Result.Ok(3);
        var d = Result.Ok(4);

        var e1 = Result.Err<int>("a");
        var e2 = Result.Err<int>("b");
        var e3 = Result.Err<int>("c");
        var e4 = Result.Err<int>("d");

        Assert.True(d > b);
        Assert.True(a < c);
        Assert.True(a <= b);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(a <= a);
        Assert.True(d >= d);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.True(d >= c);

        Assert.True(a < e1);
        Assert.True(e2 > e1);
        Assert.True(e1 >= d);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(e1 >= e1);
        Assert.True(e4 <= e4);
#pragma warning restore CS1718 // Comparison made to same variable
        Assert.True(e3 < e4);

        var items = new[] { d, e1, b, e3, e2, a, c, e4 };
        Array.Sort(items);
        Assert.Equal(new[] { a, b, c, d, e1, e2, e3, e4 }, items);
    }

    private sealed class NotSpanFormattable : IFormattable
    {
        public int Value { get; set; }

        public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
    }

    private sealed class NotFormattable
    {
        public int Value { get; set; }

#pragma warning disable CA1305 // Specify IFormatProvider
        public override string ToString() => Value.ToString();
#pragma warning restore CA1305 // Specify IFormatProvider
    }

    [Fact]
    public void CanConvertToOption()
    {
        var ok = Result.Ok(4200);
        var err = Result.Err<int>("oops");

        var okOptSome = ok.Ok();
        var okOptNone = ok.Err();
        var errOptNone = err.Ok();
        var errOptSome = err.Err();

        Assert.Equal(Option.Some(4200), okOptSome);
        Assert.Equal(Option<string>.None, okOptNone);
        Assert.Equal(Option<int>.None, errOptNone);
        Assert.Equal(Option.Some("oops"), errOptSome);
    }

    [Fact]
    public void CanTranspose()
    {
        var okSomeTest = Result.Ok(Option.Some(42));
        var okNoneTest = Result.Ok(Option<int>.None);
        var errTest = Result.Err<Option<int>>("oops");

        var okSomeExpected = Option.Some(Result.Ok(42));
        var okNoneExpected = Option<Result<int, string>>.None;
        var errExpected = Option.Some(Result.Err<int>("oops"));

        Assert.Equal(okSomeExpected, okSomeTest.Transpose());
        Assert.Equal(okNoneExpected, okNoneTest.Transpose());
        Assert.Equal(errExpected, errTest.Transpose());
    }

    [Fact]
    public void CanMap()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var mappedOk = ok.Map(x => (decimal)(x * 2));
        var mappedErr = err.Map(x => (decimal)(x * 2));

        Assert.Equal(Result.Ok<decimal>(84), mappedOk);
        Assert.Equal(Result.Err<decimal>("oops"), mappedErr);
    }

    [Fact]
    public void CanMapErr()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

#pragma warning disable CA2201 // Do not raise reserved exception types
        var mappedOk = ok.MapErr(e => new Exception(e));
        var mappedErr = err.MapErr(e => new Exception(e));
#pragma warning restore CA2201 // Do not raise reserved exception types

        Assert.Equal(Result.OkExn(42), mappedOk);
        Assert.True(mappedErr.IsErr(out var e) && e.Message == "oops");
    }

    [Fact]
    public void CanMapOr()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okResult = ok.MapOr(x => x * 2, -1);
        var errResult = err.MapOr(x => x * 2, -1);

        Assert.Equal(84, okResult);
        Assert.Equal(-1, errResult);
    }

    [Fact]
    public void CanMapOrElse()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okResult = ok.MapOrElse(x => x * 2, _ => -1);
        var errResult = err.MapOrElse(x => x * 2, _ => -1);

        Assert.Equal(84, okResult);
        Assert.Equal(-1, errResult);
    }

    [Fact]
    public void CanUnwrapOr()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okResult = ok.UnwrapOr(-1);
        var errResult = err.UnwrapOr(-1);

        Assert.Equal(42, okResult);
        Assert.Equal(-1, errResult);
    }

    [Fact]
    public void CanUnwrapOrElse()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okResult = ok.UnwrapOrElse(_ => -1);
        var errResult = err.UnwrapOrElse(_ => -1);

        Assert.Equal(42, okResult);
        Assert.Equal(-1, errResult);
    }
}
