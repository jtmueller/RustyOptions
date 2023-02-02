#if NET7_0_OR_GREATER

using System.Globalization;
using RustyOptions;
using static RustyOptions.NumericOption;

namespace RustyOptions.Tests;

public sealed class NumericOptionTests
{
    [Fact]
    public void CanPerformBasicOperations()
    {
        var none = None<int>();
        var otherNone = 0.NoneNumeric();  // value is used to determine option type, then discarded
        var someStruct = Some(42);
        var someNullableStruct = ((int?)42).AsOption();
        var someClass = "test".AsOption();
        var nullOpt = NumericOption.Create((int?)null);
        var notNullOpt = NumericOption.Create((int?)45);

        Assert.True(none.IsNone);
        Assert.True(otherNone.IsNone);
        Assert.False(none.IsSome(out _));

        Assert.True(someStruct.IsSome(out var structVal));
        Assert.Equal(42, structVal);
        Assert.False(someStruct.IsNone);

        Assert.True(someNullableStruct.IsSome(out var ns));
        Assert.Equal(42, ns);
        Assert.False(someNullableStruct.IsNone);

        Assert.True(someClass.IsSome(out var classVal));
        Assert.Equal("test", classVal);
        Assert.False(someClass.IsNone);

        Assert.True(nullOpt.IsNone);
        Assert.Equal(Some(45), notNullOpt);
    }

    [Fact]
    public void CanMap()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.Map(x => x * 2);
        var noneResult = noneInt.Map(x => x * 2);

        Assert.True(someResult.IsSome(out var val));
        Assert.Equal(84, val);
        Assert.True(noneResult.IsNone);
    }

    [Fact]
    public void CanMapOr()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.MapOr(x => x.ToString(CultureInfo.InvariantCulture), "empty");
        var noneResult = noneInt.MapOr(x => x.ToString(CultureInfo.InvariantCulture), "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
    }

    [Fact]
    public void CanMapOrElse()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.MapOrElse(x => x.ToString(CultureInfo.InvariantCulture), () => "empty");
        var noneResult = noneInt.MapOrElse(x => x.ToString(CultureInfo.InvariantCulture), () => "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
    }

    [Fact]
    public void CanMatch()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.Match(x => x.ToString(CultureInfo.InvariantCulture), () => "empty");
        var noneResult = noneInt.Match(x => x.ToString(CultureInfo.InvariantCulture), () => "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
    }

    [Fact]
    public void CanMatchWithSideEffects()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        int output = 0;
        someInt.Match(x => { output += x; }, () => { output -= 1; });
        Assert.Equal(42, output);
        noneInt.Match(x => { output += x; }, () => { output -= 1; });
        Assert.Equal(41, output);
    }

    [Fact]
    public void CanGetSpan()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        var someSpan = someInt.AsSpan();
        var noneSpan = noneInt.AsSpan();

        Assert.False(someSpan.IsEmpty);
        Assert.True(noneSpan.IsEmpty);

        Assert.Equal(1, someSpan.Length);
        Assert.Equal(0, noneSpan.Length);

        Assert.Equal(42, someSpan[0]);
    }

    [Fact]
    public void CanEnumerate()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        int value = 0;
        foreach (var x in noneInt)
        {
            value += x;
        }

        Assert.Equal(0, value);

        foreach (var x in someInt)
        {
            value += x;
        }

        Assert.Equal(42, value);

        Assert.Equal(42, someInt.AsEnumerable().FirstOrDefault());
        Assert.Equal(0, noneInt.AsEnumerable().FirstOrDefault());
    }

    [Fact]
    public void CanExpect()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        Assert.Equal(42, someInt.Expect("Needs a value"));
        var ex = Assert.Throws<InvalidOperationException>(() => _ = noneInt.Expect("Needs a value"));
        Assert.Equal("Needs a value", ex.Message);
    }

    [Fact]
    public void CanUnwrap()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        Assert.Equal(42, someInt.Unwrap());
        _ = Assert.Throws<InvalidOperationException>(() => _ = noneInt.Unwrap());
    }

    [Fact]
    public void CanUnwrapOrDefault()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        Assert.Equal(42, someInt.UnwrapOr(-1));
        Assert.Equal(0, noneInt.UnwrapOr(default));
    }

    [Fact]
    public void CanUnwrapOrElse()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        Assert.Equal(42, someInt.UnwrapOrElse(() => -1));
        Assert.Equal(-1, noneInt.UnwrapOrElse(() => -1));
    }

    [Fact]
    public void CanEquate()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someSameInt = Some(42);
        var someOtherInt = Some(4);

        Assert.Equal(someInt, someSameInt);
        Assert.NotEqual(someInt, noneInt);
        Assert.NotEqual(someInt, someOtherInt);

        Assert.True(someInt == someSameInt);
        Assert.True(someInt != noneInt);
        Assert.False(someInt == someOtherInt);

        Assert.True(someInt.Equals((object)someSameInt));
        Assert.False(someInt.Equals((object)noneInt));
        Assert.False(someInt.Equals((object)someOtherInt));

        Assert.Equal(someInt.GetHashCode(), someSameInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), noneInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), someOtherInt.GetHashCode());
    }

    [Fact]
    public void CanConvertFromResult()
    {
        var ok = Result.Ok(4200);
        var err = Result.Err<int>("oops");

        var okOptSome = ok.OkNumber();
        var okOptNone = ok.Err();
        var errOptNone = err.OkNumber();
        var errOptSome = err.Err();

        Assert.Equal(Some(8400), okOptSome * 2);
        Assert.Equal(Option<string>.None, okOptNone);
        Assert.Equal(NumericOption<int>.None, errOptNone);
        Assert.Equal(Option.Some("oops"), errOptSome);
        Assert.Equal(Some(2100), ok.OkNumber() / 2);
    }

    [Fact]
    public void CanTransformToResultVal()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        var someResult = someInt.OkOr("No value found!");
        var noneResult = noneInt.OkOr("No value found!");

        Assert.True(someResult.IsOk(out var value) && value == 42);
        Assert.True(noneResult.IsErr(out var err) && err == "No value found!");
    }

    [Fact]
    public void CanTransformToResultFunc()
    {
        var someInt = Some(42);
        var noneInt = None<int>();

        var someResult = someInt.OkOrElse(() => "No value found!");
        var noneResult = noneInt.OkOrElse(() => "No value found!");

        Assert.True(someResult.IsOk(out var value) && value == 42);
        Assert.True(noneResult.IsErr(out var err) && err == "No value found!");
    }

    [Fact]
    public void CanGetString()
    {
        var someInt = Some(4200);
        var noneInt = None<int>();

        Assert.Equal("Some(4200)", someInt.ToString());
        Assert.Equal("None", noneInt.ToString());
        Assert.Equal("Some(4,200.00)", someInt.ToString("n2", CultureInfo.InvariantCulture));
        Assert.Equal("None", noneInt.ToString("n2", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void CanFormatToSpan()
    {
        var someInt = Some(4200);
        var noneInt = None<int>();

        Span<char> buffer = stackalloc char[255];

        Assert.True(someInt.TryFormat(buffer, out int written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Some(4200)"));

        Assert.True(noneInt.TryFormat(buffer, out written, "", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("None"));

        Assert.True(someInt.TryFormat(buffer, out written, "n2", CultureInfo.InvariantCulture));
        Assert.True(buffer[..written].SequenceEqual("Some(4,200.00)"));

        Assert.False(someInt.TryFormat(Span<char>.Empty, out written, "", null));
        Assert.False(noneInt.TryFormat(Span<char>.Empty, out written, "", null));
    }

    [Fact]
    public void CanFlatten()
    {
        var someInt = Some(Some(42));
        var noneIntOuter = None<NumericOption<int>>();
        var noneIntInner = Some(None<int>());
        var someThreeLevels = Some(Some(Some(42)));

        Assert.Equal(Some(42), someInt.Flatten());
        Assert.Equal(None<int>(), noneIntOuter.Flatten());
        Assert.Equal(None<int>(), noneIntInner.Flatten());
        Assert.Equal(someInt, someThreeLevels.Flatten());
    }

    [Fact]
    public void CanFilter()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someOtherInt = Some(43);

        Assert.Equal(someInt, someInt.Filter(x => x % 2 == 0));
        Assert.Equal(noneInt, noneInt.Filter(x => x % 2 == 0));
        Assert.Equal(noneInt, someOtherInt.Filter(x => x % 2 == 0));
    }

    [Fact]
    public void CanZipWith()
    {
        var x = Some(42.0);
        var y = Some(17);

        var result = x.ZipWith(y, (a, b) => a * b);
        var result2 = x.ZipWith(None<int>(), (a, b) => a * b);
        var result3 = None<int>().ZipWith(x, (a, b) => a * b);

        Assert.Equal(Some(714.0), result);
        Assert.True(result2.IsNone);
        Assert.True(result3.IsNone);
    }

    [Fact]
    public void CanAnd()
    {
        var some = Some(42);
        var none = None<int>();

        Assert.Equal(Some(17), some.And(Some(17)));
        Assert.Equal(None<int>(), none.And(Some(17)));
        Assert.Equal(None<int>(), some.And(None<int>()));
    }

    [Fact]
    public void CanAndThen()
    {
        var some = Some(42);
        var someOther = Some(17);
        var none = None<int>();

        Assert.Equal(Some(42.0), some.AndThen(ConvertToDouble));
        Assert.Equal(None<double>(), none.AndThen(ConvertToDouble));
        Assert.Equal(None<double>(), someOther.AndThen(ConvertToDouble));

        static NumericOption<double> ConvertToDouble(int x) => x % 2 == 0 ? Some((double)x) : None<double>();
    }

    [Fact]
    public void CanOr()
    {
        var some = Some(42);
        var none = None<int>();

        Assert.Equal(some, some.Or(Some(17)));
        Assert.Equal(Some(17), none.Or(Some(17)));
    }

    [Fact]
    public void CanOrElse()
    {
        var some = Some(42);
        var none = None<int>();

        Assert.Equal(some, some.OrElse(() => Some(-17)));
        Assert.Equal(Some(-17), none.OrElse(() => Some(-17)));
    }

    [Fact]
    public void CanXor()
    {
        var sx = Some(42);
        var sy = Some(17);
        var nn = None<int>();

        Assert.Equal(nn, nn.Xor(nn));
        Assert.Equal(sy, nn.Xor(sy));
        Assert.Equal(sx, sx.Xor(nn));
        Assert.Equal(nn, sx.Xor(sy));
    }

    [Fact]
    public void CanCompare()
    {
        var a = Some(1);
        var b = Some(2);
        var c = Some(3);
        var d = Some(4);
        var n = None<int>();

#pragma warning disable CS1718 // Comparison made to same variable
        Assert.True(b > a);
        Assert.True(b >= b);
        Assert.True(c < d);
        Assert.True(c <= c);
        Assert.True(n > a);
#pragma warning restore CS1718 // Comparison made to same variable

        var items = new[] { d, b, n, c, a };
        Array.Sort(items);
        Assert.Equal(new[] { a, b, c, d, n }, items);
    }

    [Fact]
    public void CanGetValues()
    {
        var options = Enumerable.Range(1, 10)
            .Select(x => x % 2 == 0 ? Some(x) : None<int>());

        var values = options.Values().ToArray();

        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, values);
    }

    [Fact]
    public void CanConvertToOption()
    {
        Assert.Equal(Option.Some(42), NumericOption.Some(42));
    }

    [Fact]
    public void CanParse()
    {
        var input = "45001";
        var badinput = "not a number";
        var expected = Some(45_001);
        var none = None<int>();

        Assert.Equal(expected, NumericOption.Parse<int>(input, CultureInfo.InvariantCulture));
        Assert.Equal(expected, NumericOption.Parse<int>(input));
        Assert.Equal(expected, NumericOption.Parse<int>(input.AsSpan(), CultureInfo.InvariantCulture));
        Assert.Equal(expected, NumericOption.Parse<int>(input.AsSpan()));

        Assert.Equal(none, NumericOption.Parse<int>(badinput, CultureInfo.InvariantCulture));
        Assert.Equal(none, NumericOption.Parse<int>(badinput));
        Assert.Equal(none, NumericOption.Parse<int>(badinput.AsSpan(), CultureInfo.InvariantCulture));
        Assert.Equal(none, NumericOption.Parse<int>(badinput.AsSpan()));
    }
}

#endif