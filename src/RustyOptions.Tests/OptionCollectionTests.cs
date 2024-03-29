﻿using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

        var readOnlyDict = new ReadOnlyDictionary<int, string>(numsToNames);

        Assert.Equal(Some("three"), readOnlyDict.GetValueOrNone(3));

        var dictEnum = Enumerate(numsToNames);

        Assert.Equal(Some("three"), dictEnum.GetValueOrNone(3));
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
            .Select(x => (x & 1) == 0 ? Some(x) : None<int>());

        ReadOnlySpan<int> values = options.Values().ToArray();

        Assert.Equal([2, 4, 6, 8, 10], values);
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
        static bool IsEven(int x) => (x & 1) == 0;

        int[] empty = [];
        int[] notEmpty = [3, 5, 6, 7, 8, 9];
        int[] noMatches = [1, 3, 5, 7];

        Assert.Equal(None<int>(), empty.FirstOrNone(IsEven));
        Assert.Equal(Some(6), notEmpty.FirstOrNone(IsEven));
        Assert.Equal(None<int>(), noMatches.FirstOrNone(IsEven));
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
        static bool IsEven(int x) => (x & 1) == 0;

        IList<int> empty = Array.Empty<int>();
        IReadOnlyList<int> emptyReadOnly = new ReadOnlyList<int>(empty);
        IEnumerable<int> emptyEnumerable = Enumerate(empty);

        IList<int> notEmpty = new[] { 3, 5, 6, 7, 8, 9 };
        IReadOnlyList<int> notEmptyReadOnly = new ReadOnlyList<int>(notEmpty);
        IEnumerable<int> notEmptyEnumerable = Enumerate(notEmpty);

        IList<int> noMatches = new[] { 1, 3, 5, 7 };
        IReadOnlyList<int> noMatchesReadOnly = new ReadOnlyList<int>(noMatches);
        IEnumerable<int> noMatchesEnumerable = Enumerate(noMatches);

        Assert.Equal(None<int>(), empty.LastOrNone(IsEven));
        Assert.Equal(None<int>(), emptyReadOnly.LastOrNone(IsEven));
        Assert.Equal(None<int>(), emptyEnumerable.LastOrNone(IsEven));

        Assert.Equal(Some(8), notEmpty.LastOrNone(IsEven));
        Assert.Equal(Some(8), notEmptyReadOnly.LastOrNone(IsEven));
        Assert.Equal(Some(8), notEmptyEnumerable.LastOrNone(IsEven));

        Assert.Equal(None<int>(), noMatches.LastOrNone(IsEven));
        Assert.Equal(None<int>(), noMatchesReadOnly.LastOrNone(IsEven));
        Assert.Equal(None<int>(), noMatchesEnumerable.LastOrNone(IsEven));
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
        static bool IsEven(int x) => (x & 1) == 0;

        int[] empty = [];
        int[] singleWithMatch = [4];
        int[] singleNoMatch = [3];
        int[] manyWithMatch = [3, 4, 5];
        int[] manyNoMatch = [3, 5];
        int[] manyWithManyMatches = [2, 3, 4, 5, 6];

        Assert.Equal(None<int>(), empty.SingleOrNone(IsEven));
        Assert.Equal(Some(4), singleWithMatch.SingleOrNone(IsEven));
        Assert.Equal(None<int>(), singleNoMatch.SingleOrNone(IsEven));
        Assert.Equal(Some(4), manyWithMatch.SingleOrNone(IsEven));
        Assert.Equal(None<int>(), manyNoMatch.SingleOrNone(IsEven));
        Assert.Equal(None<int>(), manyWithManyMatches.SingleOrNone(IsEven));
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

    [Fact]
    public void CanSelectWhere()
    {
        string[] strings = ["1", "two", "NaN", "four", "5", "six", "7", "eight", "9", "10"];

#if NET7_0_OR_GREATER
        var ints = strings.SelectWhere(Option.Parse<int>);
#else
        var ints = strings.SelectWhere(Option.Bind<string, int>(int.TryParse));
#endif

        Assert.Equal([1, 5, 7, 9, 10], ints);
    }

    [Fact]
    public void CanPeekStack()
    {
        var stack = new Stack<int>([1, 2, 3, 4, 5]);
        var emptyStack = new Stack<int>();

        Assert.Equal(Some(5), stack.PeekOrNone());
        Assert.True(emptyStack.PeekOrNone().IsNone);
    }

    [Fact]
    public void CanPopStack()
    {
        var stack = new Stack<int>([1, 2, 3, 4, 5]);
        var emptyStack = new Stack<int>();

        Assert.Equal(Some(5), stack.PopOrNone());
        Assert.Equal(4, stack.Count);
        Assert.True(emptyStack.PopOrNone().IsNone);
    }

    [Fact]
    public void CanPeekImmutableStack()
    {
        var stack = ImmutableStack.Create([1, 2, 3, 4, 5]);
        var emptyStack = ImmutableStack<int>.Empty;

        Assert.Equal(Some(5), stack.PeekOrNone());
        Assert.True(emptyStack.PeekOrNone().IsNone);
    }

    [Fact]
    public void CanPopImmutableStack()
    {
        var stack = ImmutableStack.Create([1, 2, 3, 4, 5]);
        var emptyStack = ImmutableStack<int>.Empty;

        stack = stack.PopOrNone(out var value);
        Assert.Equal(Some(5), value);
        Assert.Equal(4, stack.Count());
        emptyStack.PopOrNone(out value);
        Assert.True(value.IsNone);
    }

    [Fact]
    public void CanPeekQueue()
    {
        var queue = new Queue<int>([1, 2, 3, 4, 5]);
        var emptyQueue = new Queue<int>();

        Assert.Equal(Some(1), queue.PeekOrNone());
        Assert.True(emptyQueue.PeekOrNone().IsNone);
    }

    [Fact]
    public void CanDequeueQueue()
    {
        var queue = new Queue<int>([1, 2, 3, 4, 5]);
        var emptyQueue = new Queue<int>();

        Assert.Equal(Some(1), queue.DequeueOrNone());
        Assert.Equal(4, queue.Count);
        Assert.True(emptyQueue.DequeueOrNone().IsNone);
    }

    [Fact]
    public void CanPeekPriorityQueue()
    {
        var queue = new PriorityQueue<int, int>([(1, 3), (2, 2), (3, 1), (4, 1), (5, 0)]);
        var emptyQueue = new PriorityQueue<int, int>();

        Assert.Equal(Some((5, 0)), queue.PeekOrNone());
        Assert.True(emptyQueue.PeekOrNone().IsNone);
    }

    [Fact]
    public void CanDequeuePriorityQueue()
    {
        var queue = new PriorityQueue<int, int>([(1, 3), (2, 2), (3, 1), (4, 1), (5, 0)]);
        var emptyQueue = new PriorityQueue<int, int>();

        Assert.Equal(Some((5, 0)), queue.DequeueOrNone());
        Assert.Equal(4, queue.Count);
        Assert.True(emptyQueue.DequeueOrNone().IsNone);
    }

    [Fact]
    public void CanPeekImmutableQueue()
    {
        var queue = ImmutableQueue.Create([1, 2, 3, 4, 5]);
        var emptyQueue = ImmutableQueue<int>.Empty;

        Assert.Equal(Some(1), queue.PeekOrNone());
        Assert.True(emptyQueue.PeekOrNone().IsNone);
    }

    [Fact]
    public void CanDequeueImmutableQueue()
    {
        var queue = ImmutableQueue.Create([1, 2, 3, 4, 5]);
        var emptyQueue = ImmutableQueue<int>.Empty;

        queue = queue.DequeueOrNone(out var value);
        Assert.Equal(Some(1), value);
        Assert.Equal(4, queue.Count());
        emptyQueue.DequeueOrNone(out value);
        Assert.True(value.IsNone);
    }

    [Fact]
    public void CanGetValueFromHashSet()
    {
        string[] words = ["zero", "one", "two", "three"];
        var set = new HashSet<string>(words);

        var found = set.GetValueOrNone("two");
        var notFound = set.GetValueOrNone("green");

        Assert.Equal(Some("two"), found);
        Assert.True(notFound.IsNone);
        Assert.True(found.IsSome(out var value) && ReferenceEquals(value, words[2]));
    }

    [Fact]
    public void CanGetValueFromSortedSet()
    {
        string[] words = ["zero", "one", "two", "three"];
        var set = new SortedSet<string>(words);

        var found = set.GetValueOrNone("two");
        var notFound = set.GetValueOrNone("green");

        Assert.Equal(Some("two"), found);
        Assert.True(notFound.IsNone);
        Assert.True(ReferenceEquals(found.Unwrap(), words[2]));
    }

    [Fact]
    public void CanGetValueFromImmutableHashSet()
    {
        string[] words = ["zero", "one", "two", "three"];
        var set = ImmutableHashSet.Create(words);

        var found = set.GetValueOrNone("two");
        var notFound = set.GetValueOrNone("green");

        Assert.Equal(Some("two"), found);
        Assert.True(notFound.IsNone);
        Assert.True(found.IsSome(out var value) && ReferenceEquals(value, words[2]));
    }

    [Fact]
    public void CanGetValueFromImmutableSortedSet()
    {
        string[] words = ["zero", "one", "two", "three"];
        var set = ImmutableSortedSet.Create(words);

        var found = set.GetValueOrNone("two");
        var notFound = set.GetValueOrNone("green");

        Assert.Equal(Some("two"), found);
        Assert.True(notFound.IsNone);
        Assert.True(found.IsSome(out var value) && ReferenceEquals(value, words[2]));
    }

    [Fact]
    public void CanTakeFromConcurrentBag()
    {
        var bag = new ConcurrentBag<int>();
        Assert.True(bag.TakeOrNone().IsNone);

        bag.Add(5);
        Assert.Equal(Some(5), bag.TakeOrNone());
        Assert.Empty(bag);
    }

    [Fact]
    public void CanPeekConcurrentBag()
    {
        var bag = new ConcurrentBag<int>();
        Assert.True(bag.PeekOrNone().IsNone);

        bag.Add(5);
        Assert.Equal(Some(5), bag.PeekOrNone());
        Assert.NotEmpty(bag);
    }

    [Fact]
    public void CanPeekConcurrentQueue()
    {
        var bag = new ConcurrentQueue<int>();
        Assert.True(bag.PeekOrNone().IsNone);

        bag.Enqueue(5);
        Assert.Equal(Some(5), bag.PeekOrNone());
        Assert.NotEmpty(bag);
    }

    [Fact]
    public void CanDequeueConcurrentQueue()
    {
        var bag = new ConcurrentQueue<int>();
        Assert.True(bag.DequeueOrNone().IsNone);

        bag.Enqueue(5);
        Assert.Equal(Some(5), bag.DequeueOrNone());
        Assert.Empty(bag);
    }

    [Fact]
    public void CanPeekConcurrentStack()
    {
        var bag = new ConcurrentStack<int>();
        Assert.True(bag.PeekOrNone().IsNone);

        bag.Push(5);
        Assert.Equal(Some(5), bag.PeekOrNone());
        Assert.NotEmpty(bag);
    }

    [Fact]
    public void CanPopConcurrentStack()
    {
        var bag = new ConcurrentStack<int>();
        Assert.True(bag.PopOrNone().IsNone);

        bag.Push(5);
        Assert.Equal(Some(5), bag.PopOrNone());
        Assert.Empty(bag);
    }

    /// <summary>y
    /// Ensures an IEnumerable can't be downcast.
    /// </summary>
    private static IEnumerable<T> Enumerate<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Most collections that implement IReadOnlyList<T> also implement
    /// IList<T>. We need this for code coverage.
    /// </summary>
    private sealed class ReadOnlyList<T>(IList<T> inner) : IReadOnlyList<T>
    {
        public T this[int index] => inner[index];

        public int Count => inner.Count;

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
    }

    /// <summary>
    /// Most dictionaries that implement IReadOnlyDictionary also implement IDictionary.
    /// This only implements IReadOnlyDictionary, for code coverage.
    /// </summary>
    private sealed class ReadOnlyDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict) : IReadOnlyDictionary<TKey, TValue>
    {
        public TValue this[TKey key] => dict[key];

        public IEnumerable<TKey> Keys => dict.Keys;
        public IEnumerable<TValue> Values => dict.Values;
        public int Count => dict.Count;

        public bool ContainsKey(TKey key) => dict.ContainsKey(key);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => dict.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
    }
}
