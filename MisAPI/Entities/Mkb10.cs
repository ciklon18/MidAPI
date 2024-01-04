using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Mkb10")]
public class Mkb10
{
    [Key] [Column("id_guid")] public Guid? IdGuid { get; set; } = Guid.NewGuid();
    [Column("id_int")] public int IdInt { get; set; }
    [Column("root_id_int")] public int? RootIdInt { get; set; }
    [Column("root_id_guid")] public Guid? RootIdGuid { get; set; }
    [Column("mkb_code")] public string MkbCode { get; set; }
    [Column("mkb_name")] public string MkbName { get; set; }
    [Column("rec_code")] public string RecCode { get; set; }
    [Column("addl_code")] public string? AddlCode { get; set; }
    [Column("date")] public DateTime? Date { get; set; }
    [Column("parent_id")] public int IdParent { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
}