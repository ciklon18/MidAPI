using System.Text.Json.Serialization;

namespace MisAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male,
    Female
}