using System.ComponentModel.DataAnnotations;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Request;

public class InspectionEditModel
{

    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Anamnesis length must be between 1 and 5000.")]
    public string Anamnesis { get; set; } = null!;

    [Required] [StringLength(5000, MinimumLength = 1, ErrorMessage = "Complaints length must be between 1 and 5000.")]
    public string Complaints { get; set; } = null!;

    [Required] [StringLength(5000, MinimumLength = 1, ErrorMessage = "Treatment length must be between 1 and 5000.")]
    public string Treatment { get; set; } = null!;

    [Required] public Conclusion Conclusion { get; set; }

    [DateValidator] public DateTime NextVisitDate { get; set; }

    [DateValidator] public DateTime DeathDate { get; set; }
    
    [Required]  public IEnumerable<DiagnosisCreateModel> Diagnoses { get; set; } = null!;
    
}

