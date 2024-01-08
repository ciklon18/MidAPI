using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MisAPI.Configurations;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Entities;

[Table("Patients")]
public class Patient
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [DateValidator]
    [Column("create_time")]
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    [StringLength(1000, MinimumLength = 1, ErrorMessage = EntityConstants.WrongPatientNameError)]
    [Column("name")]
    public required string Name { get; set; }

    [DateValidator] [Column("birthday")] public DateTime Birthday { get; set; } = DateTime.UtcNow;
    [Column("gender")] public Gender Gender { get; set; }

    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    [Column("doctor_id")] public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}