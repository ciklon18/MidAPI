using System.ComponentModel.DataAnnotations;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Request;

public class InspectionCreateModel
{
    [Required]
    [DateValidator]
    public DateTime Date { get; set; }
    [Required]
    
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Anamnesis length must be between 1 and 5000.")]
    public required string Anamnesis { get; set; } 
    [Required]
    
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Complaints length must be between 1 and 5000.")]
    public required string Complaints { get; set; }
    [Required]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Treatment length must be between 1 and 5000.")]
    public required string Treatment { get; set; }
    [Required]
    public Conclusion Conclusion { get; set; }
    
    [DateValidator]
    public DateTime? NextVisitDate { get; set; }
    
    [DateValidator]
    public DateTime? DeathDate { get; set; }
    
    public Guid PreviousInspectionId { get; set; }
    [Required]
    public IEnumerable<DiagnosisCreateModel> Diagnoses { get; set; } = null!;
    
    public IEnumerable<ConsultationCreateModel>? Consultations { get; set; } = null!;
}