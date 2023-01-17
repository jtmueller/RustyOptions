using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for using collections with <see cref="Option{T}"/>.
/// </summary>
public static class OptionCollectionExtensions
{
    // NOTE: Due to a bug in coverlet.collector, certain lines in methods involving IAsyncEnumerable
    // will show as partially-covered in code-coverage tools, even when they are fully-covered.
    // https://github.com/coverlet-coverage/coverlet/issues/1104#issuecomment-1005332269

    /// <summary>
    /// Flattens a sequence of <see cref="Option{T}"/> into a sequence containing all inner values.
    /// Empty elements are discarded.
    /// </summary>
    /// <param name="self">The sequence of options.</param>
    /// <returns>A flattened sequence of values.</returns>
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
    /// Flattens an asynchronous sequence of <see cref="Option{T}"/> into a sequence containing all inner values.
    /// Empty elements are discarded.
    /// </summary>
    /// <param name="self">The sequence of options.</param>
    /// <param name="ct">A CancellationToken that will interrupt async iteration.</param>
    /// <returns>A flattened sequence of values.</returns>
    public static async IAsyncEnumerable<T> ValuesAsync<T>(this IAsyncEnumerable<Option<T>> self, [EnumeratorCancellation] CancellationToken ct = default)
        where T : notnull
    {
        ThrowIfNull(self);

        await foreach (var option in self.WithCancellation(ct))
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
    /// Returns the first element of an asynchronous sequence if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the first element from.</param>
    /// <param name="ct">A CancellationToken that will interrupt async iteration.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the first element if present.</returns>
    public static async ValueTask<Option<T>> FirstOrNoneAsync<T>(this IAsyncEnumerable<T> self, CancellationToken ct = default)
        where T : notnull
    {
        ThrowIfNull(self);

        await using var enumerator = self.GetAsyncEnumerator(ct);
        if (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            return Option.Some(enumerator.Current);
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
    /// Returns the first element of a sequence, satisfying a specified predicate, 
    /// if such exists.
    /// </summary>
    /// <param name="self">The sequence to return the first element from.</param>
    /// <param name="predicate">The predicate to filter by.</param>
    /// <param name="ct">A CancellationToken that will interrupt async iteration.</param>
    /// <returns>An <see cref="Option{T}"/> instance containing the first matching element, if present.</returns>
    public static async ValueTask<Option<T>> FirstOrNoneAsync<T>(this IAsyncEnumerable<T> self, Func<T, bool> predicate, CancellationToken ct = default)
        where T : notnull
    {
        ThrowIfNull(self);
        ThrowIfNull(predicate);

        await foreach (var item in self.WithCancellation(ct))
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
}
