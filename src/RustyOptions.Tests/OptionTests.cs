using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
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
        var someResult = someInt.Map(x => x.ToString());
        var noneResult = noneInt.Map(x => x.ToString());

        Assert.True(someResult.IsSome(out var str));
        Assert.Equal("42", str);
        Assert.True(noneResult.IsNone);
    }

    [Fact]
    public void CanMapOr()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.MapOr(x => x.ToString(), "empty");
        var noneResult = noneInt.MapOr(x => x.ToString(), "empty");

        Assert.Equal("42", someResult);
        Assert.Equal("empty", noneResult);
    }

    [Fact]
    public void CanMapOrElse()
    {
        var someInt = Some(42);
        var noneInt = None<int>();
        var someResult = someInt.MapOrElse(x => x.ToString(), () => "empty");
        var noneResult = noneInt.MapOrElse(x => x.ToString(), () => "empty");

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
        var someOkTest = Some(Result<int, string>.Ok(42));
        var someErrTest = Some(Result<int, string>.Err("Bad things happened"));
        var noneTest = None<Result<int, string>>();

        var someOkExpected = Result<Option<int>, string>.Ok(Some(42));
        var someErrExpected = Result<Option<int>, string>.Err("Bad things happened");
        var noneExpected = Result<Option<int>, string>.Ok(None<int>());

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
    public void CanGetOptionFromDictionary()
    {
        var ParseInt = Option.Bind<string, int>(int.TryParse);

        Dictionary<int, string> numsToNames = new()
        {
            { 1, "one" },
            { 2, "two" },
            { 3, "three" },
            { 4, "four" },
            { 5, "five" }
        };

        var namesToNums = numsToNames.ToDictionary(kvp => kvp.Value, kvp => kvp.Key.ToString());

        Assert.Equal(Some("three"), numsToNames.GetOption(3));
        Assert.True(numsToNames.GetOption(7).IsNone);

        var chainResult = numsToNames
            .GetOption(4)
            .AndThen(namesToNums.GetOption)
            .AndThen(ParseInt);

        Assert.Equal(Some(4), chainResult);

        chainResult = numsToNames
            .GetOption(96)
            .AndThen(namesToNums.GetOption)
            .AndThen(ParseInt);

        Assert.True(chainResult.IsNone);

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

    [Fact]
    public void CanBindTryGetValue()
    {
        Dictionary<int, string> numsToNames = new()
        {
            { 1, "one" },
            { 2, "two" },
            { 3, "three" },
            { 4, "four" },
            { 5, "five" }
        };

        var namesToNums = numsToNames.ToDictionary(kvp => kvp.Value, kvp => kvp.Key.ToString());

        var result = numsToNames.GetOption(2)
            .AndThen(Bind<string, string>(namesToNums.TryGetValue))
            .AndThen(Bind<string, int>(int.TryParse));

        Assert.Equal(Some(2), result);
    }

#if NET7_0_OR_GREATER

    private const string s_testJson = """
        {
            "number": 3,
            "string": "test",
            "array": [1,2,3],
            "date": "2022-12-07"
        }
        """;

    [Fact]
    public void CanGetJsonNodeValue()
    {
        var obj = (JsonObject)JsonNode.Parse(s_testJson);
        
        var numVal = obj.GetPropValue<int>("number");
        var stringVal = obj.GetPropValue<string>("string");
        var arrVal = obj.GetPropValue<int[]>("array");
        var dateVal = obj.GetPropValue<DateTime>("date");
        var noVal = obj.GetPropValue<decimal>("bogus");
        var wrongVal = obj.GetPropValue<int>("string");

        Assert.Equal(Some(3), numVal);
        Assert.Equal(Some("test"), stringVal);
        Assert.True(arrVal.IsNone); // GetPropValue does not support arrays
        Assert.Equal(Some(new DateTime(2022, 12, 7)), dateVal);
        Assert.True(noVal.IsNone);
        Assert.True(wrongVal.IsNone);

        var properArrVal = obj.GetPropOption("array");
        Assert.True(properArrVal.IsSome(out var arrNode));
        Assert.True(arrNode is JsonArray jArr && jArr.Count == 3);
    }

    [Fact]
    public void CanGetJsonElementProps()
    {
        using var doc = JsonDocument.Parse(s_testJson);

        var numVal = doc.RootElement
            .GetPropOption("number")
            .AndThen(x => Option.Bind<int>(x.TryGetInt32));

        var stringVal = doc.RootElement
            .GetPropOption("string".AsSpan())
            .AndThen(x => x.GetString().Some());

        var dateVal = doc.RootElement
            .GetPropOption("date"u8)
            .AndThen(x => Option.Bind<DateTime>(x.TryGetDateTime));

        var noVal = doc.RootElement
            .GetPropOption("bogus"u8)
            .AndThen(x => Option.Bind<decimal>(x.TryGetDecimal));

        var wrongVal = doc.RootElement
            .GetPropOption("string"u8)
            .AndThen(x => Option.Bind<int>(x.TryGetInt32));

        Assert.Equal(Some(3), numVal);
        Assert.Equal(Some("test"), stringVal);
        Assert.Equal(Some(new DateTime(2022, 12, 7)), dateVal);
        Assert.True(noVal.IsNone);
        Assert.True(wrongVal.IsNone);
    }

    [Fact]
    public void CanParseStrings()
    {
        var integer = Option.Parse<int>("12345");
        var date = Option.Parse<DateTime>("2023-06-17");
        var timespan = Option.Parse<TimeSpan>("05:11:04");
        var fraction = Option.Parse<double>("3.14");
        var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965");
        var nothing = Option.Parse<int>("foo");

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
        Assert.True(nothing.IsNone);
    }

    [Fact]
    public void CanParseSpans()
    {
        var integer = Option.Parse<int>("12345".AsSpan());
        var date = Option.Parse<DateTime>("2023-06-17".AsSpan());
        var timespan = Option.Parse<TimeSpan>("05:11:04".AsSpan());
        var fraction = Option.Parse<double>("3.14".AsSpan());
        var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965".AsSpan());
        var nothing = Option.Parse<int>("foo".AsSpan());

        Assert.True(integer.IsSome(out var i) && i == 12345);
        Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
        Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
        Assert.True(fraction.IsSome(out var x) && x == 3.14);
        Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
        Assert.True(nothing.IsNone);
    }

#endif
}
