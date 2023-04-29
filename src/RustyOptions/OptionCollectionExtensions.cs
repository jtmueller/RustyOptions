using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for using collections with <see cref="Option{T}"/>.
/// </summary>
public static class OptionCollectionExtensions
{
    /// <summary>
    /// Flattens a sequence of <see cref="Option{T}"/> into a sequence containing all inner values.
    /// Empty elements are discarded.
    /// </summary>
    /// <param name="self">The sequence of options.</param>
    /// <returns>A flattened sequence of values.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static IEnumerable<T> Values<T>(this IEnumerable<Option<T>> self)
        where T : notnull
    {
        ThrowIfNull(self);

        foreach (var option in self)
        {
            if (option.IsSome(out var value))
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the dictionary as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static Option<TValue> GetValueOrNone<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> self, TKey key)
        where TValue : notnull
        where TKey : notnull
    {
        ThrowIfNull(self);

        if (self is IDictionary<TKey, TValue> dict)
        {
            return dict.TryGetValue(key, out var value)
                ? Option.Some(value)
                : default;
        }
        else if (self is IReadOnlyDictionary<TKey, TValue> readOnlyDict)
        {
            return readOnlyDict.TryGetValue(key, out var value)
                ? Option.Some(value)
                : default;
        }

        return self
            .FirstOrNone(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key))
            .Map(pair => pair.Value);
    }

    /// <summary>
    /// Returns the first element of a sequence if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the first element from.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the first element if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> self)
        where T : notnull
    {
        ThrowIfNull(self);

        if (self is IList<T> list)
        {
            return list.Count > 0 ? Option.Some(list[0]) : default;
        }
        else if (self is IReadOnlyList<T> readOnlyList)
        {
            return readOnlyList.Count > 0 ? Option.Some(readOnlyList[0]) : default;
        }
        else
        {
            using var enumerator = self.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return Option.Some(enumerator.Current);
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the first element of a sequence, satisfying a specified predicate, 
    /// if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the first element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the first matching element, if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="predicate"/> is null.</exception>
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(predicate);

        foreach (var item in self)
        {
            if (predicate(item))
            {
                return Option.Some(item);
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the last element of a sequence if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the last element from.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the last element if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static Option<T> LastOrNone<T>(this IEnumerable<T> self)
        where T : notnull
    {
        ThrowIfNull(self);

        if (self is IList<T> list)
        {
            return list.Count > 0 ? Option.Some(list[^1]) : default;
        }
        else if (self is IReadOnlyList<T> readOnlyList)
        {
            return readOnlyList.Count > 0 ? Option.Some(readOnlyList[^1]) : default;
        }
        else
        {
            using var enumerator = self.GetEnumerator();
            if (enumerator.MoveNext())
            {
                T result;
                do
                {
                    result = enumerator.Current;
                }
                while (enumerator.MoveNext());

                return Option.Some(result);
            }
        }

        return default;
    }

    /// <summary>
    /// Returns the last element of a sequence, satisfying a specified predicate, 
    /// if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the last element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the last element if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="predicate"/> is null.</exception>
    public static Option<T> LastOrNone<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(predicate);

        if (self is IList<T> list)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                var result = list[i];
                if (predicate(result))
                {
                    return Option.Some(result);
                }
            }

            return default;
        }
        else if (self is IReadOnlyList<T> readOnlyList)
        {
            for (var i = readOnlyList.Count - 1; i >= 0; --i)
            {
                var result = readOnlyList[i];
                if (predicate(result))
                {
                    return Option.Some(result);
                }
            }

            return default;
        }
        else
        {
            using var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (predicate(result))
                {
                    while (enumerator.MoveNext())
                    {
                        var element = enumerator.Current;
                        if (predicate(element))
                        {
                            result = element;
                        }
                    }

                    return Option.Some(result);
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Returns a single element from a sequence, if it exists 
    /// and is the only element in the sequence.
    /// </summary>
    /// <param name="self">The sequence to return the element from.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the element if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static Option<T> SingleOrNone<T>(this IEnumerable<T> self)
        where T : notnull
    {
        ThrowIfNull(self);

        if (self is IList<T> list)
        {
            return list.Count == 1
                ? Option.Some(list[0])
                : default;
        }
        else if (self is IReadOnlyList<T> readOnlyList)
        {
            return readOnlyList.Count == 1
                ? Option.Some(readOnlyList[0])
                : default;
        }
        else
        {
            using var enumerator = self.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default;
            }

            var result = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                return Option.Some(result);
            }
        }

        return default;
    }

    /// <summary>
    /// Returns a single element from a sequence, satisfying a specified predicate, 
    /// if it exists and is the only element in the sequence.
    /// </summary>
    /// <param name="self">The sequence to return the element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the element if present.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> or <paramref name="predicate"/> is null.</exception>
    public static Option<T> SingleOrNone<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(predicate);

        using (var enumerator = self.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (predicate(result))
                {
                    while (enumerator.MoveNext())
                    {
                        if (predicate(enumerator.Current))
                        {
                            return default;
                        }
                    }

                    return Option.Some(result);
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Returns an element at a specified position in a sequence if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the element from.</param>
    /// <param name="index">The index in the sequence.</param>
    /// <returns>An Option&lt;T&gt; instance containing the element if found.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="self"/> is null.</exception>
    public static Option<T> ElementAtOrNone<T>(this IEnumerable<T> self, int index)
        where T : notnull
    {
        ThrowIfNull(self);

        if (index >= 0)
        {
            if (self is IList<T> list)
            {
                return index < list.Count
                    ? Option.Some(list[index])
                    : default;
            }
            else if (self is IReadOnlyList<T> readOnlyList)
            {
                return index < readOnlyList.Count
                    ? Option.Some(readOnlyList[index])
                    : default;
            }
            else
            {
                using var enumerator = self.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (index == 0)
                    {
                        return Option.Some(enumerator.Current);
                    }

                    index--;
                }
            }
        }

        return default;
    }

    /// <summary>
    /// Applies a function to each element of a sequence and returns a sequence of the values inside any <c>Some</c> results.
    /// Can be used to transform and filter in a single operation, similar to applying <c>Select/Where/Select</c>,
    /// but with better performance.
    /// </summary>
    /// <typeparam name="T1">The type contained in the incoming sequence.</typeparam>
    /// <typeparam name="T2">The type contained in the resulting sequence.</typeparam>
    /// <param name="self">The incoming sequence.</param>
    /// <param name="selector">A function that takes a value from the in coming sequence, and returns an <see cref="Option{T}"/> that will have any value included in the resulting sequence.</param>
    /// <returns>A sequence containing the values for which the <paramref name="selector"/> function returns <c>Some</c>.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="self"/> or <paramref name="selector"/> is null.</exception>
    public static IEnumerable<T2> SelectWhere<T1, T2>(this IEnumerable<T1> self, Func<T1, Option<T2>> selector)
        where T1 : notnull
        where T2 : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(selector);

        foreach (var item in self)
        {
            if (selector(item).IsSome(out var value))
            {
                yield return value;
            }
        }
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the top of the <c>Stack</c> if one is present.
    /// The object is not removed from the <c>Stack</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the stack and the returned option.</typeparam>
    /// <param name="self">The stack.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the stack has any values, and <c>None</c> if the stack is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this Stack<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var result)
            ? Option.Some(result) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the top of the <c>Stack</c> if one is present.
    /// The object is removed from the <c>Stack</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the stack and the returned option.</typeparam>
    /// <param name="self">The stack.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the stack has any values, and <c>None</c> if the stack is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PopOrNone<T>(this Stack<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPop(out var result)
            ? Option.Some(result) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the top of the <c>ImmutableStack</c> if one is present.
    /// The object is not removed from the <c>ImmutableStack</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the stack and the returned option.</typeparam>
    /// <param name="self">The stack.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the stack has any values, and <c>None</c> if the stack is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this ImmutableStack<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.IsEmpty ? default : Option.Some(self.Peek());
    }

    /// <summary>
    /// Sets <paramref name="result"/> to an <see cref="Option{T}"/> that contains the object at the top of the <c>ImmutableStack</c> if one is present.
    /// A modified version of the stack without that value is returned.
    /// </summary>
    /// <typeparam name="T">The type contained by the stack and the returned option.</typeparam>
    /// <param name="self">The stack.</param>
    /// <param name="result">An <see cref="Option{T}"/> containing the value removed from the stack, if any.</param>
    /// <returns>A modified verison of the <c>ImmutableStack</c> without the removed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableStack<T> PopOrNone<T>(this ImmutableStack<T> self, out Option<T> result)
        where T : notnull
    {
        ThrowIfNull(self);

        if (self.IsEmpty)
        {
            result = default;
            return self;
        }

        var newStack = self.Pop(out var value);
        result = Option.Some(value);
        return newStack;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>Queue</c> if one is present.
    /// The object is not removed from the <c>Queue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this Queue<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var value)
            ? Option.Some(value) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>Queue</c> if one is present.
    /// The object is removed from the <c>Queue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> DequeueOrNone<T>(this Queue<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryDequeue(out var value)
            ? Option.Some(value) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>PriorityQueue</c> if one is present.
    /// The object is not removed from the <c>PriorityQueue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <typeparam name="TPriority">The type of the priority value.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<(T Element, TPriority Priority)> PeekOrNone<T, TPriority>(this PriorityQueue<T, TPriority> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var element, out var priority)
            ? Option.Some((element, priority)) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>PriorityQueue</c> if one is present.
    /// The object is removed from the <c>PriorityQueue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <typeparam name="TPriority">The type of the priority value.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<(T Element, TPriority Priority)> DequeueOrNone<T, TPriority>(this PriorityQueue<T, TPriority> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryDequeue(out var element, out var priority)
            ? Option.Some((element, priority)) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>ImmutableQueue</c> if one is present.
    /// The object is not removed from the <c>ImmutableQueue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this ImmutableQueue<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.IsEmpty ? default : Option.Some(self.Peek());
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>ImmutableQueue</c> if one is present.
    /// The object is removed from the <c>ImmutableQueue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <param name="self">The queue.</param>
    /// <param name="result">An <see cref="Option{T}"/> containing the value removed from the stack, if any.</param>
    /// <returns>A modified verison of the <c>ImmutableStack</c> without the removed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableQueue<T> DequeueOrNone<T>(this ImmutableQueue<T> self, out Option<T> result)
        where T : notnull
    {
        ThrowIfNull(self);

        if (self.IsEmpty)
        {
            result = default;
            return self;
        }

        var newQueue = self.Dequeue(out var value);
        result = Option.Some(value);
        return newQueue;
    }

    /// <summary>
    /// Searches the set for a given value and returns the equal value it finds, if any.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The HashSet.</param>
    /// <param name="equalValue">The value to search for.</param>
    /// <returns><c>Some</c> containing the value the search found, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetValueOrNone<T>(this HashSet<T> self, T equalValue)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(equalValue, out var actualValue)
            ? Option.Some(actualValue) : default;
    }

    /// <summary>
    /// Searches the set for a given value and returns the equal value it finds, if any.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The SortedSet.</param>
    /// <param name="equalValue">The value to search for.</param>
    /// <returns><c>Some</c> containing the value the search found, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetValueOrNone<T>(this SortedSet<T> self, T equalValue)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(equalValue, out var actualValue)
            ? Option.Some(actualValue) : default;
    }

    /// <summary>
    /// Searches the set for a given value and returns the equal value it finds, if any.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The HashSet.</param>
    /// <param name="equalValue">The value to search for.</param>
    /// <returns><c>Some</c> containing the value the search found, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetValueOrNone<T>(this ImmutableHashSet<T> self, T equalValue)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(equalValue, out var actualValue)
            ? Option.Some(actualValue) : default;
    }

    /// <summary>
    /// Searches the set for a given value and returns the equal value it finds, if any.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="self">The SortedSet.</param>
    /// <param name="equalValue">The value to search for.</param>
    /// <returns><c>Some</c> containing the value the search found, or <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetValueOrNone<T>(this ImmutableSortedSet<T> self, T equalValue)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(equalValue, out var actualValue)
            ? Option.Some(actualValue) : default;
    }

    /// <summary>
    /// Attempts to remove and return an object from the collection.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="self">The collection.</param>
    /// <returns>Returns a <c>Some</c> containing the value if there is a value, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> TakeOrNone<T>(this IProducerConsumerCollection<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryTake(out var item) ? Option.Some(item) : default;
    }

    /// <summary>
    /// Attempts to return an object from the <see cref="ConcurrentBag{T}"/> without removing it.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam> 
    /// <param name="self">The collection.</param>
    /// <returns>Returns a <c>Some</c> containing the value if there is a value, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this ConcurrentBag<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var item) ? Option.Some(item) : default;
    }

    /// <summary>
    /// Attempts to return an object from the beginning of the <see cref="ConcurrentQueue{T}"/> without removing it.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam> 
    /// <param name="self">The collection.</param>
    /// <returns>Returns a <c>Some</c> containing the value if there is a value, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this ConcurrentQueue<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var item) ? Option.Some(item) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the beginning of the <c>PriorityQueue</c> if one is present.
    /// The object is removed from the <c>PriorityQueue</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the queue and the returned option.</typeparam>
    /// <param name="self">The queue.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the queue has any values, and <c>None</c> if the queue is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> DequeueOrNone<T>(this ConcurrentQueue<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryDequeue(out var result)
            ? Option.Some(result) : default;
    }

    /// <summary>
    /// Attempts to return an object from the top of the <see cref="ConcurrentStack{T}"/> without removing it.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam> 
    /// <param name="self">The collection.</param>
    /// <returns>Returns a <c>Some</c> containing the value if there is a value, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PeekOrNone<T>(this ConcurrentStack<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPeek(out var item) ? Option.Some(item) : default;
    }

    /// <summary>
    /// Returns an <see cref="Option{T}"/> that contains the object at the top of the <c>Stack</c> if one is present.
    /// The object is removed from the <c>Stack</c>.
    /// </summary>
    /// <typeparam name="T">The type contained by the stack and the returned option.</typeparam>
    /// <param name="self">The stack.</param>
    /// <returns>An <see cref="Option{T}"/> that is <c>Some</c> if the stack has any values, and <c>None</c> if the stack is empty.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> PopOrNone<T>(this ConcurrentStack<T> self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryPop(out var result)
            ? Option.Some(result) : default;
    }
}
