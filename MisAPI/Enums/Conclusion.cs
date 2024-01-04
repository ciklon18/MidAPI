using System.Text.Json.Serialization;

namespace MisAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum Conclusion
{
    Disease,
    Recovery,
    Death
}