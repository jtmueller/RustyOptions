using System.Globalization;
using static RustyOptions.Option;

namespace RustyOptions.Tests;

public class OptionCollectionTests
{

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

        var namesToNums = numsToNames.ToDictionary(
            kvp => kvp.Value,
            kvp => kvp.Key.ToString(CultureInfo.InvariantCulture));

        Assert.Equal(Some("three"), numsToNames.GetOrNone(3));
        Assert.True(numsToNames.GetOrNone(7).IsNone);

        var chainResult = numsToNames
            .GetOrNone(4)
            .AndThen(namesToNums.GetOrNone)
            .AndThen(ParseInt);

        Assert.Equal(Some(4), chainResult);

        chainResult = numsToNames
            .GetOrNone(96)
            .AndThen(namesToNums.GetOrNone)
            .AndThen(ParseInt);

        Assert.True(chainResult.IsNone);

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

        var namesToNums = numsToNames.ToDictionary(kvp => kvp.Value, kvp => kvp.Key.ToString(CultureInfo.InvariantCulture));

        var result = numsToNames.GetOrNone(2)
            .AndThen(Bind<string, string>(namesToNums.TryGetValue))
            .AndThen(Bind<string, int>(int.TryParse));

        Assert.Equal(Some(2), result);
    }
}

