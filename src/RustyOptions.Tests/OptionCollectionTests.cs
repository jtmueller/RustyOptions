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

        var result = numsToNames.GetOption(2)
            .AndThen(Bind<string, string>(namesToNums.TryGetValue))
            .AndThen(Bind<string, int>(int.TryParse));

        Assert.Equal(Some(2), result);
    }
}

