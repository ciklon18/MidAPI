using System.Text.Json.Serialization;

namespace MisAPI.Enums;



[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PatientSorting
{
    NameAsc,
    NameDesc,
    CreateAsc,
    CreateDesc,
    InspectionAsc,
    InspectionDesc
}
