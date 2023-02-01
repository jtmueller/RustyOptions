using System.Globalization;
using System.Text.Json;

namespace RustyOptions.Tests;

public class JsonSerializationTests
{
    private const string DtoString = "2019-09-07T15:50:00-04:00";
    private static readonly DateTimeOffset DtoParsed = DateTimeOffset.Parse("2019-09-07T15:50:00-04:00", CultureInfo.InvariantCulture);
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [Fact]
    public void CanParseOptionsAllSome()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllSome, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Option.Some(17), parsed.Bar);
        Assert.Equal(Option.Some("Frank"), parsed.Name);
        Assert.Equal(Option.Some(DtoParsed), parsed.LastUpdated);
    }

    [Fact]
    public void CanParseOptionsAllMissing()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllMissing, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Option.None<int>(), parsed.Bar);
        Assert.Equal(Option.None<string>(), parsed.Name);
        Assert.Equal(Option.None<DateTimeOffset>(), parsed.LastUpdated);
    }

    [Fact]
    public void CanParseOptionsAllNull()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllNull, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Option.None<int>(), parsed.Bar);
        Assert.Equal(Option.None<string>(), parsed.Name);
        Assert.Equal(Option.None<DateTimeOffset>(), parsed.LastUpdated);
    }

    [Fact]
    public void CanSerializeOptionsAllSome()
    {
        var sut = new ClassWithOptions
        {
            Foo =  42,
            Bar = Option.Some(17),
            Name = Option.Some("Frank"),
            LastUpdated = Option.Some(DtoParsed)
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(OptionsAllSome, serialized);
    }

    [Fact]
    public void CanSerializeOptionsAllNone()
    {
        var sut = new ClassWithOptions
        {
            Foo = 42,
            Bar = Option.None<int>(),
            Name = Option.None<string>(),
            LastUpdated = Option.None<DateTimeOffset>()
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(OptionsAllNull, serialized);
    }

#if NET7_0_OR_GREATER

    [Fact]
    public void CanParseNumericOptionsAllSome()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithNumbers>(NumericOptionsAllSome, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(NumericOption.Some(17), parsed.Bar);
        Assert.Equal(NumericOption.Some(3.14), parsed.Baz);
        Assert.Equal(NumericOption.Some((byte)255), parsed.Quux);
    }

    [Fact]
    public void CanParseNumericOptionsAllMissing()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithNumbers>(NumericOptionsAllMissing, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(NumericOption.None<int>(), parsed.Bar);
        Assert.Equal(NumericOption.None<double>(), parsed.Baz);
        Assert.Equal(NumericOption.None<byte>(), parsed.Quux);
    }

    [Fact]
    public void CanParseNumericOptionsAllNull()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithNumbers>(NumericOptionsAllNull, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(NumericOption.None<int>(), parsed.Bar);
        Assert.Equal(NumericOption.None<double>(), parsed.Baz);
        Assert.Equal(NumericOption.None<byte>(), parsed.Quux);
    }

    [Fact]
    public void CanSerializeNumericOptionsAllSome()
    {
        var sut = new ClassWithNumbers
        {
            Foo = 42,
            Bar = NumericOption.Some(17),
            Baz = NumericOption.Some(3.14),
            Quux = NumericOption.Some((byte)255)
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(NumericOptionsAllSome, serialized);
    }

    [Fact]
    public void CanSerializeNumericOptionsAllNone()
    {
        var sut = new ClassWithNumbers
        {
            Foo = 42,
            Bar = NumericOption.None<int>(),
            Baz = NumericOption.None<double>(),
            Quux = NumericOption.None<byte>()
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(NumericOptionsAllNull, serialized);
    }

#endif

    [Fact]
    public void CanParseResultOk()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultOk, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Result.Ok(75), parsed.CurrentCount);
    }

    [Fact]
    public void CanParseResultErr()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultErr, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Result.Err<int>("not found!"), parsed.CurrentCount);
    }

    [Fact]
    public void CanParseResultMissing()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultMissing, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);

        // A default instance of the Result struct will be in the Err state, but the err value will be
        // the default value for TErr, and therefore possibly null.
        Assert.True(parsed.CurrentCount.IsErr(out var err) && err is null);
    }

    [Fact]
    public void CanParseResultNull()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ClassWithResult>(ResultNull, JsonOpts));
    }

    [Fact]
    public void CanSerializeResultOk()
    {
        var sut = new ClassWithResult
        {
            Foo = 42,
            CurrentCount = Result.Ok(75)
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(ResultOk, serialized);
    }

    [Fact]
    public void CanSerializeResultErr()
    {
        var sut = new ClassWithResult
        {
            Foo = 42,
            CurrentCount = Result.Err<int>("not found!")
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(ResultErr, serialized);
    }

    [Fact]
    public void CanSerializeUnit()
    {
        Unit unit;

        var serialized = JsonSerializer.Serialize(unit);

        Assert.Equal("null", serialized);
    }

    [Fact]
    public void CanDeserializeUnit()
    {
        var deserialized = JsonSerializer.Deserialize<Unit>("null");

        Assert.Equal(Unit.Default, deserialized);
    }

    [Fact]
    public void CanSerializeUnitResult()
    {
        var ok = Result.Ok(Unit.Default);
        var err = Result.Err<Unit>("oops");

        var serOk = JsonSerializer.Serialize(ok);
        var serErr = JsonSerializer.Serialize(err);

        Assert.Equal("""{"ok":null}""", serOk);
        Assert.Equal("""{"err":"oops"}""", serErr);
    }

    [Fact]
    public void CanDeserializeUnitResult()
    {
        var ok = Result.Ok(Unit.Default);
        var err = Result.Err<Unit>("oops");

        var desOk = JsonSerializer.Deserialize<Result<Unit, string>>("""{"ok":null}""");
        var desErr = JsonSerializer.Deserialize<Result<Unit, string>>("""{"err":"oops"}""");

        Assert.Equal(ok, desOk);
        Assert.Equal(err, desErr);
    }

    private const string OptionsAllSome = $$"""
        {"foo":42,"bar":17,"name":"Frank","lastUpdated":"{{DtoString}}"}
        """;

    private const string OptionsAllMissing = """
        { "foo": 42 }
        """;

    private const string OptionsAllNull = """
        {"foo":42,"bar":null,"name":null,"lastUpdated":null}
        """;

    private const string NumericOptionsAllSome = """
        {"foo":42,"bar":17,"baz":3.14,"quux":255}
        """;

    private const string NumericOptionsAllMissing = """
        { "foo": 42 }
        """;

    private const string NumericOptionsAllNull = """
        {"foo":42,"bar":null,"baz":null,"quux":null}
        """;

    private const string ResultOk = """
        {"foo":42,"currentCount":{"ok":75}}
        """;

    private const string ResultErr = """
        {"foo":42,"currentCount":{"err":"not found!"}}
        """;

    private const string ResultMissing = """
        { "foo": 42 }
        """;

    private const string ResultNull = """
        { "foo": 42, "currentCount": null }
        """;

    private sealed record ClassWithOptions
    {
        public int Foo { get; set; }
        public Option<int> Bar { get; set; }
        public Option<string> Name { get; set; }
        public Option<DateTimeOffset> LastUpdated { get; set; }
    }

    private sealed record ClassWithResult
    {
        public int Foo { get; set; }
        public Result<int, string> CurrentCount { get; set; }
    }

#if NET7_0_OR_GREATER
    private sealed record ClassWithNumbers
    {
        public int Foo { get; set; }
        public NumericOption<int> Bar { get; set; }
        public NumericOption<double> Baz { get; set; }
        public NumericOption<byte> Quux { get; set; }
    }
#endif
}

