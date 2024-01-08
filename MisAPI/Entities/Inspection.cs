using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MisAPI.Enums;

namespace MisAPI.Entities;

[Table("Inspections")]
public class Inspection
{
    [Key] [Column("id")] public required Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; }
    [Column("date")] public DateTime Date { get; set; }
    [Column("anamnesis")] public string? Anamnesis { get; set; } = string.Empty;
    [Column("complaints")] public string? Complaints { get; set; } = string.Empty;
    [Column("treatment")] public string? Treatment { get; set; } = string.Empty;
    [Column("conclusion")] public Conclusion Conclusion { get; set; }
    [Column("next_visit_date")] public DateTime? NextVisitDate { get; set; }
    [Column("death_date")] public DateTime? DeathDate { get; set; }
    [Column("base_inspection_id")] public Guid? BaseInspectionId { get; set; }
    [Column("previous_inspection_id")] public Guid? PreviousInspectionId { get; set; }
    [Column("patient_id")] public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    [Column("doctor_id")] public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    [Column("diagnoses")] public ICollection<Diagnosis>? Diagnoses { get; set; } = new List<Diagnosis>();
    [Column("consultations")] public ICollection<Consultation>? Consultations { get; set; } = new List<Consultation>();
}