﻿#if NET7_0_OR_GREATER

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

    [Fact]
    public void CanGetModulus()
    {
        Assert.True(Some(42) % 2 == 0);
        Assert.Equal(Some(1), 97 % Some(2));
        Assert.True((None<int>() % 2).IsNone);
        Assert.True((97 % None<int>()).IsNone);
    }

    [Fact]
    public void CanIncrement()
    {
        var last = None<int>();

        Assert.Equal(None<int>(), ++last);

        for (var i = Some(0); i < 5; i++)
        {
            Assert.True(last.IsNone || i > last);
            last = i;
        }
    }

    [Fact]
    public void CanDecrement()
    {
        var last = None<int>();

        Assert.Equal(None<int>(), --last);

        for (var i = Some(5); i >= 0; i--)
        {
            Assert.True(last.IsNone || i < last);
            last = i;
        }
    }

    [Fact]
    public void CanUseUnaryPlus()
    {
        Assert.Equal(Some(-42), +Some(-42));
        Assert.Equal(Some(42), +Some(42));
        Assert.Equal(None<int>(), +None<int>());
    }

    [Fact]
    public void CanUseUnaryMinus()
    {
        Assert.Equal(Some(-42), -Some(42));
        Assert.Equal(Some(42), -Some(-42));
        Assert.Equal(None<int>(), -None<int>());
    }

    [Fact]
    public void CanUseGenericZeroAndOne()
    {
        var total = NumericOption<int>.Zero;
        for (var i = NumericOption<int>.Zero; i < 5; i += NumericOption<int>.One)
        {
            total += i;
        }

        Assert.Equal(Some(10), total);
    }
}

#endif
