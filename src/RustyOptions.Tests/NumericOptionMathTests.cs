#if NET7_0_OR_GREATER

using System.Globalization;
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

    [Fact]
    public void IsCanonical()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsCanonical(val);
        
        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsCanonical(val));
        }
    }

    [Fact]
    public void IsComplexNumber()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsComplexNumber(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsComplexNumber(val));
        }
    }

    [Fact]
    public void IsEvenInteger()
    {
        RunTest(Some(96), GetExpected(96));
        RunTest(Some(17), GetExpected(17));
        RunTest(Some(4.8), GetExpected(4.8));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsEvenInteger(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsEvenInteger(val));
        }
    }

    [Fact]
    public void IsFinite()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.PositiveInfinity), GetExpected(double.PositiveInfinity));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsFinite(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsFinite(val));
        }
    }

    [Fact]
    public void IsImaginaryNumber()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.PositiveInfinity), GetExpected(double.PositiveInfinity));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsImaginaryNumber(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsImaginaryNumber(val));
        }
    }

    [Fact]
    public void IsInfinity()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.PositiveInfinity), GetExpected(double.PositiveInfinity));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsInfinity(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsInfinity(val));
        }
    }

    [Fact]
    public void IsInteger()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsInteger(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsInteger(val));
        }
    }

    [Fact]
    public void IsNaN()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.NaN), GetExpected(double.NaN));
        RunTest(None<int>(), true);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsNaN(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsNaN(val));
        }
    }

    [Fact]
    public void IsNegative()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(-3.8), GetExpected(-3.8));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsNegative(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsNegative(val));
        }
    }

    [Fact]
    public void IsNegativeInfinity()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.PositiveInfinity), GetExpected(double.PositiveInfinity));
        RunTest(Some(double.NegativeInfinity), GetExpected(double.NegativeInfinity));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsNegativeInfinity(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsNegativeInfinity(val));
        }
    }

    [Fact]
    public void IsNormal()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsNormal(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsNormal(val));
        }
    }

    [Fact]
    public void IsOddInteger()
    {
        RunTest(Some(96), GetExpected(96));
        RunTest(Some(17), GetExpected(17));
        RunTest(Some(4.8), GetExpected(4.8));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsOddInteger(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsOddInteger(val));
        }
    }

    [Fact]
    public void IsPositive()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(-3.8), GetExpected(-3.8));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsPositive(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsPositive(val));
        }
    }

    [Fact]
    public void IsPositiveInfinity()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(double.PositiveInfinity), GetExpected(double.PositiveInfinity));
        RunTest(Some(double.NegativeInfinity), GetExpected(double.NegativeInfinity));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsPositiveInfinity(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsPositiveInfinity(val));
        }
    }

    [Fact]
    public void IsRealNumber()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsRealNumber(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsRealNumber(val));
        }
    }

    [Fact]
    public void IsSubnormal()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsSubnormal(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsSubnormal(val));
        }
    }

    [Fact]
    public void IsZero()
    {
        RunTest(Some(97), GetExpected(97));
        RunTest(Some(0), GetExpected(0));
        RunTest(Some(3.9), GetExpected(3.9));
        RunTest(None<int>(), false);

        static bool GetExpected<T>(T val) where T : INumber<T>
            => T.IsZero(val);

        static void RunTest<T>(T val, bool expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.IsZero(val));
        }
    }

    [Fact]
    public void MaxMagnitude()
    {
        RunTest(Some(12), Some(1001), GetExpected(12, 1001));
        RunTest(Some(0), Some(-100), GetExpected(0, -100));
        RunTest(Some(3.9), Some(501.4), GetExpected(3.9, 501.4));
        RunTest(None<int>(), Some(50), None<int>());

        static T GetExpected<T>(T val1, T val2) where T : INumber<T>
            => T.MaxMagnitude(val1, val2);

        static void RunTest<T>(T val1, T val2, T expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.MaxMagnitude(val1, val2));
        }
    }

    [Fact]
    public void MaxMagnitudeNumber()
    {
        RunTest(Some(12), Some(1001), GetExpected(12, 1001));
        RunTest(Some(0), Some(-100), GetExpected(0, -100));
        RunTest(Some(3.9), Some(501.4), GetExpected(3.9, 501.4));
        RunTest(None<int>(), Some(50), None<int>());

        static T GetExpected<T>(T val1, T val2) where T : INumber<T>
            => T.MaxMagnitudeNumber(val1, val2);

        static void RunTest<T>(T val1, T val2, T expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.MaxMagnitudeNumber(val1, val2));
        }
    }

    [Fact]
    public void MinMagnitude()
    {
        RunTest(Some(12), Some(1001), GetExpected(12, 1001));
        RunTest(Some(0), Some(-100), GetExpected(0, -100));
        RunTest(Some(3.9), Some(501.4), GetExpected(3.9, 501.4));
        RunTest(None<int>(), Some(50), None<int>());

        static T GetExpected<T>(T val1, T val2) where T : INumber<T>
            => T.MinMagnitude(val1, val2);

        static void RunTest<T>(T val1, T val2, T expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.MinMagnitude(val1, val2));
        }
    }

    [Fact]
    public void MinMagnitudeNumber()
    {
        RunTest(Some(12), Some(1001), GetExpected(12, 1001));
        RunTest(Some(0), Some(-100), GetExpected(0, -100));
        RunTest(Some(3.9), Some(501.4), GetExpected(3.9, 501.4));
        RunTest(None<int>(), Some(50), None<int>());

        static T GetExpected<T>(T val1, T val2) where T : INumber<T>
            => T.MinMagnitudeNumber(val1, val2);

        static void RunTest<T>(T val1, T val2, T expected) where T : INumber<T>
        {
            Assert.Equal(expected, T.MinMagnitudeNumber(val1, val2));
        }
    }

    [Fact]
    public void CanParseSpanStyleProvider()
    {
        Assert.Equal(Some(Parse<int>("512")), Parse<NumericOption<int>>("512"));
        Assert.Equal(None<int>(), Parse<NumericOption<int>>("not a number"));

        static T Parse<T>(string value) where T : INumber<T>
        {
            return T.Parse(value.AsSpan(), NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }

    [Fact]
    public void CanParseStringStyleProvider()
    {
        Assert.Equal(Some(Parse<int>("512")), Parse<NumericOption<int>>("512"));
        Assert.Equal(None<int>(), Parse<NumericOption<int>>("not a number"));

        static T Parse<T>(string value) where T : INumber<T>
        {
            return T.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }

    [Fact]
    public void CanParseSpanProvider()
    {
        Assert.Equal(Some(Parse<int>("512")), Parse<NumericOption<int>>("512"));
        Assert.Equal(None<int>(), Parse<NumericOption<int>>("not a number"));

        static T Parse<T>(string value) where T : INumber<T>
        {
            return T.Parse(value.AsSpan(), CultureInfo.InvariantCulture);
        }
    }

    [Fact]
    public void CanParseStringProvider()
    {
        Assert.Equal(Some(Parse<int>("512")), Parse<NumericOption<int>>("512"));
        Assert.Equal(None<int>(), Parse<NumericOption<int>>("not a number"));

        static T Parse<T>(string value) where T : INumber<T>
        {
            return T.Parse(value, CultureInfo.InvariantCulture);
        }
    }

    [Fact]
    public void CanTryParseSpanStyleProvider()
    {
        Assert.Equal(Parse<int>("512"), Parse<NumericOption<int>>("512"));
        Assert.Equal((false, None<int>()), Parse<NumericOption<int>>("not a number"));

        static (bool, T) Parse<T>(string value) where T : INumber<T>
        {
            return (T.TryParse(value.AsSpan(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result), result!);
        }
    }

    [Fact]
    public void CanTryParseStringStyleProvider()
    {
        Assert.Equal(Parse<int>("512"), Parse<NumericOption<int>>("512"));
        Assert.Equal((false, None<int>()), Parse<NumericOption<int>>("not a number"));

        static (bool, T) Parse<T>(string value) where T : INumber<T>
        {
            return (T.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result), result!);
        }
    }

    [Fact]
    public void CanTryParseSpanProvider()
    {
        Assert.Equal(Parse<int>("512"), Parse<NumericOption<int>>("512"));
        Assert.Equal((false, None<int>()), Parse<NumericOption<int>>("not a number"));

        static (bool, T) Parse<T>(string value) where T : INumber<T>
        {
            return (T.TryParse(value.AsSpan(), CultureInfo.InvariantCulture, out var result), result!);
        }
    }

    [Fact]
    public void CanTryParseStringProvider()
    {
        Assert.Equal(Parse<int>("512"), Parse<NumericOption<int>>("512"));
        Assert.Equal((false, None<int>()), Parse<NumericOption<int>>("not a number"));

        static (bool, T) Parse<T>(string value) where T : INumber<T>
        {
            return (T.TryParse(value, CultureInfo.InvariantCulture, out var result), result!);
        }
    }
}

#endif
