using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MisAPI.Converters;

namespace MisAPI.Entities;

[Table("Specialities")]
public class Speciality
{
    public Speciality(string name)
    {
        Name = name;
    }

    [Key] [Column("id")] public Guid Id { get; init; }

    [Column("name")]
    [Required]
    [StringLength(maximumLength: 100)]
    public string Name { get; set; }


    [Column("create_time")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime CreateTime { get; set; }

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}