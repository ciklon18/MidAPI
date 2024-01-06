using System.Text.Json.Serialization;

namespace MisAPI.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]

public enum DiagnosisType
{
    Main,
    Concomitant,
    Complication 
}