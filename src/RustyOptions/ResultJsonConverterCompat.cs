using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.ArgumentNullException;

namespace RustyOptions;

/// <summary>
/// Supports <see cref="Result{T, TErr}"/> in System.Text.Json serialization. 
/// Produces JSON compatible with TypeScript type:
/// <code>type Result&lt;T, TErr&gt;{ ok: T } | { err: TErr }</code>
/// </summary>
internal sealed class ResultJsonConverterCompat : JsonConverterFactory
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
        Type valueType = genericArgs[0];
        Type errType = genericArgs[1];

        var converter = Activator.CreateInstance(
            typeof(ResultJsonConverterCompatInner<,>).MakeGenericType([valueType, errType]),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: [options],
            culture: null
        ) as JsonConverter;

        return converter;
    }

    private sealed class ResultJsonConverterCompatInner<T, TErr> : JsonConverter<Result<T, TErr>>
        where T : notnull
    {
        private readonly JsonConverter<T> _valueConverter;
        private readonly JsonConverter<TErr> _errConverter;
        private readonly Type _valueType;
        private readonly Type _errType;

        public ResultJsonConverterCompatInner(JsonSerializerOptions options)
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
                output = Result.Ok<T, TErr>(_valueConverter.Read(ref reader, _valueType, options)!);
            }
            else if (reader.ValueSpan.SequenceEqual("err"u8) && reader.Read())
            {
                output = Result.Err<T, TErr>(_errConverter.Read(ref reader, _errType, options)!);
            }
            else
            {
                throw new NotSupportedException($"Unable to read property: '{reader.GetString()}'");
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
                _valueConverter.Write(writer, val, options);
            }
            else
            {
                writer.WritePropertyName("err"u8);
                _errConverter.Write(writer, value.UnwrapErr(), options);
            }

            writer.WriteEndObject();
        }
    }
}
