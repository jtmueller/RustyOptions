using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.ArgumentNullException;

namespace RustyOptions;

// TODO: Update README when this is implemented, including docs for backwards compat mode.

/// <summary>
/// Supports <see cref="Result{T, TErr}"/> in System.Text.Json serialization.
/// Produces JSON compatible with TypeScript discriminated union:
/// <code>
/// type Result&lt;T, TErr&gt; = { ok: true; value: T } | { ok: false; error: TErr };
/// </code>
/// </summary>
internal sealed class ResultJsonConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        ThrowIfNull(typeToConvert);

        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(Result<,>);
    }

    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        ThrowIfNull(typeToConvert);
        ThrowIfNull(options);

        var genericArgs = typeToConvert.GetGenericArguments();
        var converter = Activator.CreateInstance(
            typeof(ResultJsonConverterInner<,>).MakeGenericType(genericArgs),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [options],
            culture: null
        ) as JsonConverter;

        return converter;
    }

    private sealed class ResultJsonConverterInner<T, TErr> : JsonConverter<Result<T, TErr>>
        where T : notnull
    {
        private readonly JsonConverter<T> _valueConverter;
        private readonly JsonConverter<TErr> _errConverter;
        private readonly Type _valueType;
        private readonly Type _errType;

        public ResultJsonConverterInner(JsonSerializerOptions options) : base()
        {
            // Cache the types.
            _valueType = typeof(T);
            _errType = typeof(TErr);

            // For performance, use the existing converter.
            _valueConverter = (JsonConverter<T>)options.GetConverter(_valueType);
            _errConverter = (JsonConverter<TErr>)options.GetConverter(_errType);
        }

        public override Result<T, TErr> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            Result<T, TErr> output;

            if (reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read())
            {
                if (reader.TokenType == JsonTokenType.True)
                {
                    if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("value"u8) && reader.Read())
                    {
                        output = Result.Ok<T, TErr>(_valueConverter.Read(ref reader, _valueType, options)!);
                    }
                    else
                    {
                        throw new JsonException($"Unable to read 'value' property when ok is true: '{reader.GetString()}'");
                    }
                }
                else if (reader.TokenType == JsonTokenType.False)
                {
                    if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("error"u8) && reader.Read())
                    {
                        output = Result.Err<T, TErr>(_errConverter.Read(ref reader, _errType, options)!);
                    }
                    else
                    {
                        throw new JsonException($"Unable to read 'error' property when ok is false: '{reader.GetString()}'");
                    }
                }
                else
                {
                    throw new JsonException($"Unable to read 'ok' property: '{reader.GetString()}'");
                }
            }
            else if (reader.ValueSpan.SequenceEqual("value"u8) && reader.Read())
            {
                var value = _valueConverter.Read(ref reader, _valueType, options);

                if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read() && reader.TokenType == JsonTokenType.True)
                {
                    output = Result.Ok<T, TErr>(value!);
                }
                else
                {
                    throw new JsonException($"Unable to find 'ok' property with a true value when value is present: '{reader.GetString()}'");
                }
            }
            else if (reader.ValueSpan.SequenceEqual("error"u8) && reader.Read())
            {
                var err = _errConverter.Read(ref reader, _errType, options);

                if (reader.Read() && reader.TokenType == JsonTokenType.PropertyName && reader.ValueSpan.SequenceEqual("ok"u8) && reader.Read() && reader.TokenType == JsonTokenType.False)
                {
                    output = Result.Err<T, TErr>(err!);
                }
                else
                {
                    throw new JsonException($"Unable to find 'ok' property with a false value when error is present: '{reader.GetString()}'");
                }
            }
            else
            {
                throw new JsonException($"Unable to read property: '{reader.GetString()}'");
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException();

            return output;
        }

        public override void Write(Utf8JsonWriter writer, Result<T, TErr> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.IsOk(out var val))
            {
                writer.WritePropertyName("ok"u8);
                writer.WriteBooleanValue(true);
                writer.WritePropertyName("value"u8);
                _valueConverter.Write(writer, val, options);
            }
            else
            {
                writer.WritePropertyName("ok"u8);
                writer.WriteBooleanValue(false);
                writer.WritePropertyName("error"u8);
                _errConverter.Write(writer, value.UnwrapErr(), options);
            }

            writer.WriteEndObject();
        }
    }
}
