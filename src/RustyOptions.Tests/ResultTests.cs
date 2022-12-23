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

        Assert.True(Result.Try(Throws).IsErr(out var ex)
            && ex is IndexOutOfRangeException);
        Assert.True(Result.Try(DoesNotThrow).IsOk(out var val) && val == 42);
    }
}
