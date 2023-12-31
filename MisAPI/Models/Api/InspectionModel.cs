using MisAPI.Validator;

namespace MisAPI.Models.Api;

using System.ComponentModel.DataAnnotations;

public class InspectionModel
{
    [Required] public Guid Id { get; set; }
    [Required] [DateValidator] public DateTime CreateTime { get; set; }
    [DateValidator] public DateTime Date { get; set; }

    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Anamnesis length must be between 1 and 5000.")]
    public string Anamnesis { get; set; } = null!;

    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Complaints length must be between 1 and 5000.")]
    public string Complaints { get; set; } = null!;

    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Treatment length must be between 1 and 5000.")]
    public string Treatment { get; set; } = null!;

    // public Conclusion Conclusion { get; set; }

    [DateValidatorAttribute] public DateTime NextVisitDate { get; set; }

    [DateValidatorAttribute] public DateTime DeathDate { get; set; }

    public Guid BaseInspectionId { get; set; }
    
    public Guid PreviousInspectionId { get; set; }
    
    // public PatientModel Patient { get; set; } = null!;
    
    public DoctorModel Doctor { get; set; } = null!;

    // public IEnumerable<DiagnosisCreateModel> Diagnoses { get; set; } = null!;
    //
    // public IEnumerable<ConsultationCreateModel> Consultations { get; set; } = null!;
}