using System.ComponentModel.DataAnnotations;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Api;

public class InspectionPreviewModel
{
    public Guid Id { get; set; }
    
    [DateValidator] public DateTime CreateTime { get; set; }
    
    public Guid PreviousId { get; set; }

    [DateValidator] public DateTime Date { get; set; }

    public Conclusion Conclusion { get; set; }

    public Guid DoctorId { get; set; }

    [StringLength(maximumLength: 1000, MinimumLength = 1, ErrorMessage = "Doctor length must be between 1 and 1000.")]

    public string Doctor { get; set; } = null!;

    public Guid PatientId { get; set; }

    [StringLength(maximumLength: 1000, MinimumLength = 1, ErrorMessage = "Patient length must be between 1 and 1000.")]

    public string Patient { get; set; } = null!;

    public DiagnosisModel? Diagnosis { get; set; } = null!;

    public bool HasChain { get; set; }

    public bool HasNested { get; set; }
}