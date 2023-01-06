#if NET7_0_OR_GREATER

using System.Numerics;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
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
        RunTest(Some(5), Some(10));
        RunTest(Some(5.0), Some(10.0));

        static void RunTest<T>(T limit, T expected) where T : INumber<T>
        {
            var total = T.Zero;
            for (var i = T.Zero; i < limit; i += T.One)
            {
                total += i;
            }

            Assert.Equal(expected, total);
        }
    }

    [Fact]
    public void CanUseOtherStaticProperties()
    {
        var iex = GetExpected<int>();
        RunTest(iex.Radix, Some(iex.AdditiveIdentity), Some(iex.MultiplicativeIdentity));

        var dex = GetExpected<double>();
        RunTest(dex.Radix, Some(dex.AdditiveIdentity), Some(dex.MultiplicativeIdentity));

        static (int Radix, T AdditiveIdentity, T MultiplicativeIdentity) GetExpected<T>() where T : INumber<T>
            => (T.Radix, T.AdditiveIdentity, T.MultiplicativeIdentity);

        static void RunTest<T>(int radix, T additive, T multiplicative) where T : INumber<T>
        {
            Assert.Equal(radix, T.Radix);
            Assert.Equal(additive, T.AdditiveIdentity);
            Assert.Equal(multiplicative, T.MultiplicativeIdentity);
        }
    }

    [Fact]
    public void CanGetAbsoluteValue()
    {
        Assert.Equal(Some(42), NumericOption<int>.Abs(Some(-42)));
    }

    [Fact]
    public void CanGetMinMax()
    {
        var a = Some(42);
        var b = Some(-12);
        var none = None<int>();

        Assert.Equal(a, NumericOption<int>.Max(a, b));
        Assert.Equal(b, NumericOption<int>.Min(a, b));
        Assert.Equal(a, NumericOption<int>.Max(a, none));
        Assert.Equal(b, NumericOption<int>.Max(none, b));
        Assert.Equal(a, NumericOption<int>.Min(a, none));
        Assert.Equal(b, NumericOption<int>.Min(none, b));
        Assert.Equal(none, NumericOption<int>.Max(none, none));
        Assert.Equal(none, NumericOption<int>.Min(none, none));
    }

    [Fact]
    public void CanClamp()
    {
        var bigValue = Some(500);
        var smallValue = Some(1);
        var min = Some(50);
        var max = Some(100);
        var none = None<int>();

        Assert.Equal(max, NumericOption<int>.Clamp(bigValue, min, max));
        Assert.Equal(min, NumericOption<int>.Clamp(smallValue, min, max));
        Assert.Equal(none, NumericOption<int>.Clamp(none, min, max));
        Assert.Equal(none, NumericOption<int>.Clamp(bigValue, none, max));
        Assert.Equal(none, NumericOption<int>.Clamp(bigValue, min, none));
    }
}

#endif
