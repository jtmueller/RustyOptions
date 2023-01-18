#if NET7_0_OR_GREATER

using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Supports <see cref="NumericOption{T}"/> in System.Text.Json serialization.
/// </summary>
public sealed class NumericOptionJsonConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        ThrowIfNull(typeToConvert);

        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(NumericOption<>);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ThrowIfNull(typeToConvert);
        ThrowIfNull(options);

        Type valueType = typeToConvert.GetGenericArguments()[0];

        var converter = Activator.CreateInstance(
            typeof(NumericOptionJsonConverterInner<>).MakeGenericType(new[] { valueType }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null
        ) as JsonConverter;

        return converter;
    }

    private sealed class NumericOptionJsonConverterInner<T> : JsonConverter<NumericOption<T>>
        where T : struct, INumber<T>
    {
        private readonly JsonConverter<T> _valueConverter;
        private readonly Type _valueType;

        public NumericOptionJsonConverterInner(JsonSerializerOptions options)
        {
            // Cache the value type.
            _valueType = typeof(T);

            // For performance, use the existing converter.
            _valueConverter = (JsonConverter<T>)options.GetConverter(_valueType);
        }

        public override NumericOption<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Null => default,
                _ => NumericOption.Some(_valueConverter.Read(ref reader, _valueType, options)!)
            };
        }

        public override void Write(Utf8JsonWriter writer, NumericOption<T> value, JsonSerializerOptions options)
        {
            if (value.IsSome(out var val))
            {
                _valueConverter.Write(writer, val, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}

#endif
