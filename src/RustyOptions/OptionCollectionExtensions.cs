using System;
using System.Runtime.CompilerServices;
using static System.ArgumentNullException;

namespace RustyOptions;

public static class OptionCollectionExtensions
{
    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(key, out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue(key, out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/> from the <see cref="IDictionary{TKey, TValue}"/> as an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="self">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns>If the key is found, returns <c>Some(value)</c>. Otherwise, <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TValue> GetOption<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
        where TValue : notnull where TKey : notnull
    {
        // This overload is needed to disambiguate between IDictionary and IReadOnlyDictionary,
        // as Dictionary implements both.
        ThrowIfNull(self);

        return self.TryGetValue(key, out var value)
            ? Option.Some(value)
            : default;
    }
}

