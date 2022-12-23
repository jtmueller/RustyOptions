using System.Collections;
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

        Assert.Equal(Some("three"), numsToNames.GetValueOrNone(3));
        Assert.True(numsToNames.GetValueOrNone(7).IsNone);

        var chainResult = numsToNames.GetValueOrNone(4)
            .AndThen(namesToNums.GetValueOrNone)
            .AndThen(ParseInt);

        Assert.Equal(Some(4), chainResult);

        chainResult = numsToNames.GetValueOrNone(96)
            .AndThen(namesToNums.GetValueOrNone)
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

        var result = numsToNames.GetValueOrNone(2)
            .AndThen(Bind<string, string>(namesToNums.TryGetValue))
            .AndThen(Bind<string, int>(int.TryParse));

        Assert.Equal(Some(2), result);
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
    public void CanGetFirst()
    {
        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IList<int> notEmpty = new[] { 3, 4, 5 };
        IReadOnlyList<int> notEmptyReadOnly = new ReadOnlyList<int>(notEmpty);
        IEnumerable<int> notEmptyEnumerable = Enumerate(notEmpty);

        Assert.Equal(None<int>(), empty.FirstOrNone());
        Assert.Equal(None<int>(), emptyReadOnly.FirstOrNone());
        Assert.Equal(None<int>(), emptyEnumerable.FirstOrNone());

        Assert.Equal(Some(3), notEmpty.FirstOrNone());
        Assert.Equal(Some(3), notEmptyReadOnly.FirstOrNone());
        Assert.Equal(Some(3), notEmptyEnumerable.FirstOrNone());
    }

    [Fact]
    public void CanGetFirstMatch()
    {
        static bool Predicate(int x) => x % 2 == 0;

        var empty = Array.Empty<int>();
        var notEmpty = new[] { 3, 5, 6, 7, 8, 9 };
        var noMatches = new[] { 1, 3, 5, 7 };

        Assert.Equal(None<int>(), empty.FirstOrNone(Predicate));
        Assert.Equal(Some(6), notEmpty.FirstOrNone(Predicate));
        Assert.Equal(None<int>(), noMatches.FirstOrNone(Predicate));
    }

    [Fact]
    public void CanGetLast()
    {
        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IList<int> notEmpty = new[] { 3, 4, 5 };
        IReadOnlyList<int> notEmptyReadOnly = new ReadOnlyList<int>(notEmpty);
        IEnumerable<int> notEmptyEnumerable = Enumerate(notEmpty);

        Assert.Equal(None<int>(), empty.LastOrNone());
        Assert.Equal(None<int>(), emptyReadOnly.LastOrNone());
        Assert.Equal(None<int>(), emptyEnumerable.LastOrNone());

        Assert.Equal(Some(5), notEmpty.LastOrNone());
        Assert.Equal(Some(5), notEmptyReadOnly.LastOrNone());
        Assert.Equal(Some(5), notEmptyEnumerable.LastOrNone());
    }

    [Fact]
    public void CanGetLastMatch()
    {
        static bool Predicate(int x) => x % 2 == 0;

        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IList<int> notEmpty = new[] { 3, 5, 6, 7, 8, 9 };
        IReadOnlyList<int> notEmptyReadOnly = new ReadOnlyList<int>(notEmpty);
        IEnumerable<int> notEmptyEnumerable = Enumerate(notEmpty);

        IList<int> noMatches = new[] { 1, 3, 5, 7 };
        IReadOnlyList<int> noMatchesReadOnly = new ReadOnlyList<int>(noMatches);
        IEnumerable<int> noMatchesEnumerable = Enumerate(noMatches);

        Assert.Equal(None<int>(), empty.LastOrNone(Predicate));
        Assert.Equal(None<int>(), emptyReadOnly.LastOrNone(Predicate));
        Assert.Equal(None<int>(), emptyEnumerable.LastOrNone(Predicate));

        Assert.Equal(Some(8), notEmpty.LastOrNone(Predicate));
        Assert.Equal(Some(8), notEmptyReadOnly.LastOrNone(Predicate));
        Assert.Equal(Some(8), notEmptyEnumerable.LastOrNone(Predicate));

        Assert.Equal(None<int>(), noMatches.LastOrNone(Predicate));
        Assert.Equal(None<int>(), noMatchesReadOnly.LastOrNone(Predicate));
        Assert.Equal(None<int>(), noMatchesEnumerable.LastOrNone(Predicate));
    }

    [Fact]
    public void CanGetSingle()
    {
        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IList<int> oneItem = new[] { 5 };
        IReadOnlyList<int> oneItemReadOnly = new ReadOnlyList<int>(oneItem);
        IEnumerable<int> oneItemEnumerable = Enumerate(oneItem);

        IList<int> manyItems = new[] { 5, 6, 7 };
        IReadOnlyList<int> manyItemsReadOnly = new ReadOnlyList<int>(manyItems);
        IEnumerable<int> manyItemsEnumerable = Enumerate(manyItems);

        Assert.Equal(None<int>(), empty.SingleOrNone());
        Assert.Equal(None<int>(), emptyReadOnly.SingleOrNone());
        Assert.Equal(None<int>(), emptyEnumerable.SingleOrNone());

        Assert.Equal(Some(5), oneItem.SingleOrNone());
        Assert.Equal(Some(5), oneItemReadOnly.SingleOrNone());
        Assert.Equal(Some(5), oneItemEnumerable.SingleOrNone());

        Assert.Equal(None<int>(), manyItems.SingleOrNone());
        Assert.Equal(None<int>(), manyItemsReadOnly.SingleOrNone());
        Assert.Equal(None<int>(), manyItemsEnumerable.SingleOrNone());
    }

    [Fact]
    public void CanGetSingleMatch()
    {
        static bool Predicate(int x) => x % 2 == 0;

        var empty = Array.Empty<int>();
        var singleWithMatch = new[] { 4 };
        var singleNoMatch = new[] { 3 };
        var manyWithMatch = new[] { 3, 4, 5 };
        var manyNoMatch = new[] { 3, 5 };
        var manyWithManyMatches = new[] { 2, 3, 4, 5, 6 };

        Assert.Equal(None<int>(), empty.SingleOrNone(Predicate));
        Assert.Equal(Some(4), singleWithMatch.SingleOrNone(Predicate));
        Assert.Equal(None<int>(), singleNoMatch.SingleOrNone(Predicate));
        Assert.Equal(Some(4), manyWithMatch.SingleOrNone(Predicate));
        Assert.Equal(None<int>(), manyNoMatch.SingleOrNone(Predicate));
        Assert.Equal(None<int>(), manyWithManyMatches.SingleOrNone(Predicate));
    }

    [Fact]
    public void CanGetElementAt()
    {
        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IEnumerable<int> notEmptyEnumerable = Enumerable.Range(0, 10);
        IList<int> notEmpty = notEmptyEnumerable.ToList();
        IReadOnlyList<int> notEmptyReadOnly = new ReadOnlyList<int>(notEmpty);

        Assert.Equal(None<int>(), empty.ElementAtOrNone(-1));

        Assert.Equal(None<int>(), empty.ElementAtOrNone(5000));
        Assert.Equal(None<int>(), emptyReadOnly.ElementAtOrNone(5000));
        Assert.Equal(None<int>(), emptyEnumerable.ElementAtOrNone(5000));

        Assert.Equal(None<int>(), notEmpty.ElementAtOrNone(5000));
        Assert.Equal(None<int>(), notEmptyReadOnly.ElementAtOrNone(5000));
        Assert.Equal(None<int>(), notEmptyEnumerable.ElementAtOrNone(5000));

        Assert.Equal(None<int>(), empty.ElementAtOrNone(5));
        Assert.Equal(None<int>(), emptyReadOnly.ElementAtOrNone(5));
        Assert.Equal(None<int>(), emptyEnumerable.ElementAtOrNone(5));

        Assert.Equal(Some(5), notEmpty.ElementAtOrNone(5));
        Assert.Equal(Some(5), notEmptyReadOnly.ElementAtOrNone(5));
        Assert.Equal(Some(5), notEmptyEnumerable.ElementAtOrNone(5));
    }

    private static IEnumerable<T> Enumerate<T>(IList<T> list)
    {
        foreach (var item in list)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Most collections that implement IReadOnlyList<T> also implement
    /// IList<T>. We need this for code coverage.
    /// </summary>
    private sealed class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _inner;

        public ReadOnlyList(IList<T> inner) => _inner = inner;

        public T this[int index] => _inner[index];

        public int Count => _inner.Count;

        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _inner.GetEnumerator();
    }
}

