#if NET7_0_OR_GREATER

using System.Numerics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RustyOptions;

/// <summary>
/// This class contains static methods for creation an <see cref="Option{T}"/>.
/// </summary>
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Not concerned with Visual Basic.")]
public static class NumericOption
{
    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Some<T>(T value) where T : struct, INumber<T> => new(value);

    /// <summary>
    /// Returns a <c>Some</c> option for the specified <paramref name="value"/> is it is not null, otherwise <c>None</c>.
    /// </summary>
    /// <param name="value">The value to wrap in a <c>Some</c> option.</param>
    /// <returns>The given value, wrapped in a <c>Some</c> option.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Create<T>(T? value)
        where T : struct, INumber<T>
    {
        return value.HasValue
            ? new(value.GetValueOrDefault())
            : NumericOption<T>.None;
    }

    /// <summary>
    /// Returns the <c>None</c> option for the specified <typeparamref name="T"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> None<T>() where T : struct, INumber<T> => default;

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Parse<T>(string s, IFormatProvider? provider)
        where T : struct, IParsable<T>, INumber<T>
    {
        return T.TryParse(s, provider, out var value)
            ? new(value) : default;
    }

    /// <summary>
    /// Parses a string into any type that supports <see cref="IParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the string into.</typeparam>
    /// <param name="s">The string to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Parse<T>(string s) where T : struct, IParsable<T>, INumber<T>
        => Parse<T>(s, provider: null);

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <param name="provider">An optional format provider.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Parse<T>(ReadOnlySpan<char> s, IFormatProvider? provider)
        where T : struct, ISpanParsable<T>, INumber<T>
    {
        return T.TryParse(s, provider, out var value)
            ? new(value) : default;
    }

    /// <summary>
    /// Parses a char span into any type that supports <see cref="ISpanParsable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to parse the char span into.</typeparam>
    /// <param name="s">The char span to parse.</param>
    /// <returns>The parsed value wrapped in a <c>Some</c> option, or else <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericOption<T> Parse<T>(ReadOnlySpan<char> s) where T : struct, ISpanParsable<T>, INumber<T>
        => Parse<T>(s, provider: null);
}

#endif