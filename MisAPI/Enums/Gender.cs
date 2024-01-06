using System.Text.Json.Serialization;
using MisAPI.Converters;

namespace MisAPI.Enums;

[JsonConverter(typeof(GenderConverter))]
public enum Gender
{
    Male,
    Female
}