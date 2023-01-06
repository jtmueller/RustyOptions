#if NET7_0_OR_GREATER

using System.Globalization;
using static RustyOptions.NumericOption;

namespace RustyOptions.Tests;

public class NumericOptionMathTests
{
    [Fact]
    public void CanAdd()
    {
        var a = Some(3);
        var b = Some(5);
        var c = Some(4.5);
        var none = None<int>();

        Assert.Equal(Some(8), a + b);
        Assert.Equal(Some(8), a + 5);
        Assert.Equal(Some(4), 1 + a);
        Assert.True((a + none).IsNone);
        Assert.True((none + 5).IsNone);
        Assert.Equal(Some(7.5), a.Map(x => (double)x) + c);
    }

    [Fact]
    public void CanSubtract()
    {
        var a = Some(3);
        var b = Some(5);
        var c = Some(4.5);
        var none = None<int>();

        Assert.Equal(Some(2), b - a);
        Assert.Equal(Some(2), b - 3);
        Assert.Equal(Some(2), 5 - a);
        Assert.True((a - none).IsNone);
        Assert.True((none - 5).IsNone);
        Assert.Equal(Some(-1.5), a.Map(x => (double)x) - c);
    }

    [Fact]
    public void CanMultiply()
    {
        var a = Some(3);
        var b = Some(5);
        var c = Some(4.5);
        var none = None<int>();

        Assert.Equal(Some(15), a * b);
        Assert.Equal(Some(15), a * 5);
        Assert.Equal(Some(6), 2 * a);
        Assert.True((a * none).IsNone);
        Assert.True((none * 5).IsNone);
        Assert.Equal(Some(9.0), 2.0 * c);
    }

    [Fact]
    public void CanDivide()
    {
        var a = Some(3);
        var b = Some(15);
        var c = Some(9.0);
        var none = None<int>();

        Assert.Equal(Some(5), b / a);
        Assert.Equal(Some(5), 15 / a);
        Assert.Equal(Some(3), b / 5);
        Assert.True((a / none).IsNone);
        Assert.True((none / 5).IsNone);
        Assert.Equal(Some(4.5), c / 2.0);
    }
}

#endif
