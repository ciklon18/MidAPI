using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Mkb10Roots")]
public class Mkb10Root
{
    [Key] [Column("key_id")] public int KeyId { get; set; }
    [Column("code")] public string Code { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}