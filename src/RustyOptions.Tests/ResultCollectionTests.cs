using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class ResultCollectionTests
{
    [Fact]
    public void CanGetValues()
    {
        var options = Enumerable.Range(1, 10)
            .Select(x => x % 2 == 0 ? Ok(x) : Err<int>("odd"));

        var values = options.Values().ToArray();

        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, values);
    }

    [Fact]
    public void CanGetErrors()
    {
        var options = Enumerable.Range(1, 10)
            .Select(x => x % 2 == 0 ? Ok(x) : Err<int>("odd"));

        var errors = options.Errors().ToArray();

        Assert.Equal(Enumerable.Repeat("odd", 5), errors);
    }
}

