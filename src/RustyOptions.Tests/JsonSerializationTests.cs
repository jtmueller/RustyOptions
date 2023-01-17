using System.Globalization;
using System.Text.Json;
using static RustyOptions.Option;
using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class JsonSerializationTests
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [Fact]
    public void CanParseOptionsAllSome()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithOptions>(OptionsAllSome, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Some(17), parsed.Bar);
        Assert.Equal(Some("Frank"), parsed.Name);
        Assert.Equal(Some(DateTimeOffset.Parse("2019-09-07T15:50-04:00", CultureInfo.InvariantCulture)), parsed.LastUpdated);
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
        Assert.Equal(Err<int>(""), parsed.CurrentCount);
    }

    [Fact]
    public void CanParseResultNull()
    {
        var parsed = JsonSerializer.Deserialize<ClassWithResult>(ResultNull, JsonOpts);

        Assert.NotNull(parsed);
        Assert.Equal(42, parsed.Foo);
        Assert.Equal(Err<int>(""), parsed.CurrentCount);
    }

    private const string OptionsAllSome = """
        { "foo": 42, "bar": 17, "name": "Frank", "lastUpdated": "2019-09-07T15:50-04:00" }
        """;

    private const string OptionsAllMissing = """
        { "foo": 42 }
        """;

    private const string OptionsAllNull = """
        { "foo": 42, "bar": null, "name": null, "lastUpdated": null }
        """;

    private const string ResultOk = """
        { "foo": 42, "currentCount": 75 }
        """;

    private const string ResultErr = """
        { "foo": 42, "currentCount": "not found!" }
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

