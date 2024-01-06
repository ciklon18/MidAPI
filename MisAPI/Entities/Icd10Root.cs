using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Icd10Roots")]
public class Icd10Root
{
    public Icd10Root(int keyId, string? code, string name, Guid id)
    {
        KeyId = keyId;
        Code = code;
        Name = name;
        Id = id;
    }

    [Key] [Column("key_id")] public int KeyId { get; set; }
    [Column("code")] public string? Code { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}