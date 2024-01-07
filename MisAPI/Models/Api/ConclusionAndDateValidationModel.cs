using MisAPI.Enums;

namespace MisAPI.Models.Api;

public record ConclusionAndDateValidationModel(Conclusion Conclusion, DateTime? DeathDate, DateTime? NextVisitDate)
{
    public ConclusionAndDateValidationModel() : this(Conclusion.Disease, DateTime.UtcNow, DateTime.UtcNow)
    {
    }
}