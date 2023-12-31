
using System.ComponentModel.DataAnnotations;
using MisAPI.Validator;

namespace MisAPI.Models.Response;

public class InspectionPreviewModel
{
    [Required] public Guid Id { get; set; }
    
    [Required] [DateValidator] public DateTime CreateTime { get; set; }
    
    public Guid? PreviousId { get; set; }
    
    [Required] [DateValidator] public DateTime Date { get; set; }
    
    // [Required] public Conclusion Conclusion { get; set; }
    
    [Required] public Guid DoctorId { get; set; }
    [Required] public string Doctor { get; set; } = null!;

    [Required] public Guid PatientId { get; set; }
    [Required] public string Patient { get; set; } = null!;
    

    // [Required] public DiagnosisModel Diagnosis { get; set; }

    public bool HasChain { get; set; }
    public bool hasNested { get; set; }
}