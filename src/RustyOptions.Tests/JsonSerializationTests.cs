using System.Globalization;
using System.Text.Json;
using static RustyOptions.Option;
using static RustyOptions.Result;

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
        Assert.Equal(Some(17), parsed.Bar);
        Assert.Equal(Some("Frank"), parsed.Name);
        Assert.Equal(Some(DtoParsed), parsed.LastUpdated);
    }

    [Fact]
    public void CanParseOptionsAllMissing()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllMissing, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(None<int>(), parsed.Bar);
        Assert.Equal(None<string>(), parsed.Name);
        Assert.Equal(None<DateTimeOffset>(), parsed.LastUpdated);
    }

    [Fact]
    public void CanParseOptionsAllNull()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllNull, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(None<int>(), parsed.Bar);
        Assert.Equal(None<string>(), parsed.Name);
        Assert.Equal(None<DateTimeOffset>(), parsed.LastUpdated);
    }

    [Fact]
    public void CanSerializeOptionsAllSome()
    {
        var sut = new ClassWithOptions
        {
            Foo =  42,
            Bar = Some(17),
            Name = Some("Frank"),
            LastUpdated = Some(DtoParsed)
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
            Bar = None<int>(),
            Name = None<string>(),
            LastUpdated = None<DateTimeOffset>()
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(OptionsAllNull, serialized);
    }

    [Fact]
    public void CanParseResultOk()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultOk, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Ok(75), parsed.CurrentCount);
    }

    [Fact]
    public void CanParseResultErr()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultErr, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Err<int>("not found!"), parsed.CurrentCount);
    }

    [Fact]
    public void CanParseResultMissing()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultMissing, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
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
            CurrentCount = Ok(75)
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
            CurrentCount = Err<int>("not found!")
        };

        var serialized = JsonSerializer.Serialize(sut, JsonOpts);

        Assert.Equal(ResultErr, serialized);
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
}

