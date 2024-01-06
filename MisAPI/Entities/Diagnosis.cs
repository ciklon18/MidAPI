using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MisAPI.Enums;

namespace MisAPI.Entities;

[Table("Diagnoses")]
public class Diagnosis
{
    [Key] [Column("id")] public Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; }
    [Column("code")] public string? Code { get; set; } = string.Empty;
    [Column("name")] public string Name { get; set; } = string.Empty;
    [Column("description")] public string? Description { get; set; } = string.Empty;
    [Column("type")] public DiagnosisType Type { get; set; }
    [Column("inspection_id")] public Guid InspectionId { get; set; }
    public Inspection Inspection { get; set; } = null!;
    [Column("icd_diagnosis_id")] public Guid IcdDiagnosisId { get; set; }

}