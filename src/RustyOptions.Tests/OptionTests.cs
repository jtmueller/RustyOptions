using System.Globalization;
using static RustyOptions.Option;

namespace RustyOptions.Tests;

public sealed class OptionTests
{
    [Fact]
    public void CanPerformBasicOperations()
    {
        var none = None<int>();
        var someStruct = Some(42);
        var someNullableStruct = ((int?)42).Some();
        var someClass = "test".Some();
        var nullOptClass = Option.Create((string?)null);
        var nullOptStruct = Option.Create((int?)null);

        Assert.True(none.IsNone);
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

        Assert.True(nullOptClass.IsNone);
        Assert.True(nullOptStruct.IsNone);
    }

    [Fact]
    public void CanMap()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.Map(x => x.ToString(CultureInfo.InvariantCulture));
        var noneResult = noneInt.Map(x => x.ToString(CultureInfo.InvariantCulture));

        Assert.True(someResult.IsSome(out var str));
        Assert.Equal("42", str);
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

        Assert.True(((object)someInt).Equals(someSameInt));
        Assert.False(((object)someInt).Equals(noneInt));
        Assert.False(((object)someInt).Equals(someOtherInt));

        Assert.Equal(someInt.GetHashCode(), someSameInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), noneInt.GetHashCode());
        Assert.NotEqual(someInt.GetHashCode(), someOtherInt.GetHashCode());
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
    public void CanTranspose()
    {
        var someOkTest = Some(Result.Ok<int, string>(42));
        var someErrTest = Some(Result.Err<int, string>("Bad things happened"));
        var noneTest = None<Result<int, string>>();

        var someOkExpected = Result.Ok<Option<int>, string>(Some(42));
        var someErrExpected = Result.Err<Option<int>, string>("Bad things happened");
        var noneExpected = Result.Ok<Option<int>, string>(None<int>());

        Assert.Equal(someOkExpected, someOkTest.Transpose());
        Assert.Equal(someErrExpected, someErrTest.Transpose());
        Assert.Equal(noneExpected, noneTest.Transpose());
    }

    [Fact]
    public void CanGetString()
    {
        var someInt = Some(4200);
        var noneInt = None<int>();

        Assert.Equal("Some(4200)", someInt.ToString());
        Assert.Equal("None", noneInt.ToString());
        Assert.Equal("Some(4,200.00)", someInt.ToString("n2", CultureInfo.InvariantCulture));
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
    }

    [Fact]
    public void CanFlatten()
    {
        var someInt = Some(Some(42));
        var noneIntOuter = None<Option<int>>();
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
    public void CanZip()
    {
        var x = Some(42);
        var y = Some(17);

        var result = x.Zip(y);
        var result2 = x.Zip(None<int>());
        var result3 = None<int>().Zip(x);

        Assert.True(result.IsSome(out var value) && value == (42, 17));
        Assert.True(result2.IsNone);
        Assert.True(result3.IsNone);
    }

    [Fact]
    public void CanZipWith()
    {
        var x = Some("key");
        var y = Some(17);

        var result = x.ZipWith(y, KeyValuePair.Create);
        var result2 = x.ZipWith(None<int>(), KeyValuePair.Create);
        var result3 = None<int>().ZipWith(x, KeyValuePair.Create);

        Assert.True(result.IsSome(out var value));
        Assert.Equal("key", value.Key);
        Assert.Equal(17, value.Value);
        Assert.True(result2.IsNone);
        Assert.True(result3.IsNone);
    }

    [Fact]
    public void CanAnd()
    {
        var someStr = Some("42");
        var noneStr = None<string>();

        Assert.Equal(Some(17), someStr.And(Some(17)));
        Assert.Equal(None<int>(), noneStr.And(Some(17)));
        Assert.Equal(None<int>(), someStr.And(None<int>()));
    }

    [Fact]
    public void CanAndThen()
    {
        var someIntStr = Some("42");
        var someOtherStr = Some("foo");
        var noneStr = None<string>();

        Assert.Equal(Some(42), someIntStr.AndThen(ParseInt));
        Assert.Equal(None<int>(), noneStr.AndThen(ParseInt));
        Assert.Equal(None<int>(), someOtherStr.AndThen(ParseInt));

        static Option<int> ParseInt(string s)
            => int.TryParse(s, out int parsed) ? Some(parsed) : None<int>();
    }

    [Fact]
    public void CanOr()
    {
        var someStr = Some("42");
        var noneStr = None<string>();

        Assert.Equal(someStr, someStr.Or(Some("other")));
        Assert.Equal(Some("other"), noneStr.Or(Some("other")));
    }

    [Fact]
    public void CanOrElse()
    {
        var someStr = Some("42");
        var noneStr = None<string>();

        Assert.Equal(someStr, someStr.OrElse(() => Some("other")));
        Assert.Equal(Some("other"), noneStr.OrElse(() => Some("other")));
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
        Assert.True(n < a);
#pragma warning restore CS1718 // Comparison made to same variable

        var items = new[] { d, b, n, c, a };
        Array.Sort(items);
        Assert.Equal(new[] { n, a, b, c, d }, items);
    }

    [Fact]
    public void CanDeconstruct()
    {
        var (isSome, value) = Some(42);
        Assert.True(isSome);
        Assert.Equal(42, value);

        (isSome, value) = None<int>();
        Assert.False(isSome);
        Assert.Equal(default, value);
    }
}
