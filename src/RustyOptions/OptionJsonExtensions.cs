using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Extension methods for using <c>System.Text.Json</c> types with <see cref="Option{T}"/>.
/// </summary>
public static class OptionJsonExtensions
{
    /// <summary>
    /// Attempts to get the underlying JSON value as the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns><c>Some(value)</c> if the underlying value could be successfully converted, otherwise <c>None</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> GetOption<T>(this JsonValue self)
        where T : notnull
    {
        ThrowIfNull(self);
        return self.TryGetValue<T>(out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Attempts to get a property with the given name as the given data type. Does not support array properties - use GetPropOption for arrays.
    /// </summary>
    /// <typeparam name="T">The value type to convert to.</typeparam>
    /// <param name="self">The JsonObject to get the property from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>None</c> if the property was not present or could not be parsed into the indicated type. Otherwise, <c>Some(value)</c>.</returns>
    public static Option<T> GetPropValue<T>(this JsonObject self, string propertyName)
        where T : notnull
    {
        ThrowIfNull(self);
        if (self.TryGetPropertyValue(propertyName, out var node))
        {
            return node switch
            {
                JsonValue val => val.GetOption<T>(),
                JsonArray => default, // JsonArray isn't supported for this use
                JsonObject obj => obj.AsValue().GetOption<T>(),
                JsonNode n => n.AsValue().GetOption<T>(),
                _ => default
            };
        }

        return default;
    }

    /// <summary>
    /// Gets the indicated propery node as an <see cref="Option{T}"/>.
    /// </summary>
    /// <param name="self">The JSON node to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>None</c> if the property was not present, otherwise <c>Some(node)</c></returns>
    public static Option<JsonNode> GetPropOption(this JsonObject self, string propertyName)
    {
        ThrowIfNull(self);
        if (self.TryGetPropertyValue(propertyName, out var node))
        {
            return Option.Some(node!);
        }

        return default;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, string propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, ReadOnlySpan<char> propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option.Some(value)
            : default;
    }

    /// <summary>
    /// Gets the indicated property element as an <see cref="Option{T}"/>
    /// </summary>
    /// <param name="self">The json element to read from.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns><c>Some(JsonElement)</c> if the property was found, otherwise <c>None</c>.</returns>
    public static Option<JsonElement> GetPropOption(this JsonElement self, ReadOnlySpan<byte> propertyName)
    {
        ThrowIfNull(self);
        return self.TryGetProperty(propertyName, out var value)
            ? Option.Some(value)
            : default;
    }
}

