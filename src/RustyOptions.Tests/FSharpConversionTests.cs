using RustyOptions.FSharp;

namespace RustyOptions.Tests;

public sealed class FSharpConversionTests
{
    [Fact]
    public void CanConvertOption()
    {
        var some = Option.Some(42);
        var none = Option.None<int>();

        var someFs = some.AsFSharpOption();
        var noneFs = none.AsFSharpOption();

        Assert.Equal(42, someFs.Value);
        Assert.Throws<NullReferenceException>(() => noneFs.Value);
    }

    [Fact]
    public void CanConvertValueOption()
    {
        var some = Option.Some(42);
        var none = Option.None<int>();

        var someFs = some.AsFSharpValueOption();
        var noneFs = none.AsFSharpValueOption();

        Assert.Equal(42, someFs.Value);
        Assert.True(noneFs.IsNone);
    }

    [Fact]
    public void CanConvertResult()
    {
        var ok = Result.Ok(42);
        var err = Result.Err<int>("oops");

        var okFs = ok.AsFSharpResult();
        var errFs = err.AsFSharpResult();

        Assert.Equal(42, okFs.ResultValue);
        Assert.Equal("oops", errFs.ErrorValue);
    }
}
