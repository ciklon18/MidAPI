using System.ComponentModel.DataAnnotations;
using MisAPI.Enums;

namespace MisAPI.Models.Api;

public class DiagnosisModel
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name length must be between 1 and 200.")]
    public string Code { get; set; }
    
    [StringLength(3000, MinimumLength = 1, ErrorMessage = "Description length must be between 1 and 3000.")]
    public string? Description { get; set; }
    
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name length must be between 1 and 200.")]
    public string? Name { get; set; }
    
    public DiagnosisType Type { get; set; }
    
}