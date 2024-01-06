using System.ComponentModel.DataAnnotations;
using MisAPI.Enums;

namespace MisAPI.Models.Request;

public class DiagnosisCreateModel
{
    [Required]
    public Guid IcdDiagnosisId { get; set; }
    [StringLength(5000, ErrorMessage = "Description length must be less than 5000.")]
    public string? Description { get; set; } = null!;
    public DiagnosisType Type { get; set; }
}