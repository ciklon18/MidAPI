using System.Text.Json;
using System.Text.Json.Serialization;
using MisAPI.Enums;
using MisAPI.Exceptions;

namespace MisAPI.Converters;

public class GenderConverter : JsonConverter<Gender>
{
    public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string but got {reader.TokenType}.");
        }

        var enumString = reader.GetString();

        if (Enum.TryParse(enumString, out Gender result))
        {
            return result;
        }

        throw new IncorrectGenderException($"Unable to convert '{enumString}' to {nameof(Gender)}.");
    }

    public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}