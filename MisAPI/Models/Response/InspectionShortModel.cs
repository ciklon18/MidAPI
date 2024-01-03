using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public class InspectionShortModel
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime Date { get; set; }
    public DiagnosisModel? Diagnosis { get; set; } = null!;
}