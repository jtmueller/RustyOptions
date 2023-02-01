using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RustyOptions;

/// <summary>
/// A unit type is a type that allows only one value and holds no information. It can be used
/// as a placeholder in a <see cref="Result{T, TErr}"/> that returns no data.
/// </summary>
/// <remarks>
/// <para>The unit type is similar to <c>void</c> in C#, except that it is an actual value that
/// can be returned, and a type that can be used as a generic type parameters.</para>
/// <para>For RustyOptions, the main use of this type is to allow for <c>Result</c>-returning methods
/// that do not return a value, but might return an error: <c>Result&lt;Unit, TErr&gt;</c></para>
/// </remarks>
[Serializable]
[JsonConverter(typeof(UnitJsonConverter))]
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, ISpanFormattable
{
    /// <summary>
    /// Returns the <c>Unit</c> instance.
    /// </summary>
    public static readonly Unit Default = default;

    /// <inheritdoc />
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider) => "()";

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if ("()".AsSpan().TryCopyTo(destination))
        {
            charsWritten = 2;
            return true;
        }

        charsWritten = 0;
        return false;
    }

#pragma warning disable IDE0060 // Remove unused parameter

    /// <summary>
    /// The equality operator for <c>Unit</c>. Always returns <c>true</c>.
    /// </summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// The inequality operator for <c>Unit</c>. Always returns <c>false</c>.
    /// </summary>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// The less-than operator for <c>Unit</c>. Always returns <c>false</c>.
    /// </summary>
    public static bool operator <(Unit left, Unit right) => false;

    /// <summary>
    /// The less-than-or-equals operator for <c>Unit</c>. Always returns <c>true</c>.
    /// </summary>
    public static bool operator <=(Unit left, Unit right) => true;

    /// <summary>
    /// The greater-than operator for <c>Unit</c>. Always returns <c>false</c>.
    /// </summary>
    public static bool operator >(Unit left, Unit right) => false;

    /// <summary>
    /// The greater-than-or-equal operator for <c>Unit</c>. Always returns <c>true</c>.
    /// </summary>
    public static bool operator >=(Unit left, Unit right) => true;

    /// <summary>
    /// The addition operator for <c>Unit</c>.
    /// </summary>
    public static Unit operator +(Unit left, Unit right) => default;

    /// <summary>
    /// Provides implicit conversion between <c>Unit</c> and the empty <c>ValueTuple</c>.
    /// </summary>
    public static implicit operator ValueTuple(Unit unit) => default;

    /// <summary>
    /// Provides implicit conversion between <c>Unit</c> and the empty <c>ValueTuple</c>.
    /// </summary>
    public static implicit operator Unit(ValueTuple tuple) => default;

#pragma warning restore IDE0060 // Remove unused parameter

}
