using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Icd10")]
public class Icd10
{
    public Icd10(int idInt, int? rootIdInt, Guid? rootIdGuid, string icdCode, string icdName, string recCode,
        string addlCode, DateTime? date, int idParent)
    {
        IdInt = idInt;
        RootIdInt = rootIdInt;
        RootIdGuid = rootIdGuid;
        IcdCode = icdCode;
        IcdName = icdName;
        RecCode = recCode;
        AddlCode = addlCode;
        Date = date;
        IdParent = idParent;
    }

    [Key] [Column("id_guid")] public Guid? IdGuid { get; init; } = Guid.NewGuid();
    [Column("id_int")] public int IdInt { get; init; }
    [Column("root_id_int")] public int? RootIdInt { get; init; }
    [Column("root_id_guid")] public Guid? RootIdGuid { get; init; }
    [Column("icd_code")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Icd code must be more 0 and less 5000 characters.")]

    public string IcdCode { get; init; }
    [Column("icd_name")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Icd name must be more 0 and less 5000 characters.")]
    public string IcdName { get; init; }

    [Column("rec_code")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Rec code must be more 0 and less 5000 characters.")]
    public string RecCode { get; init; }

    [Column("addl_code")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Addl code must be more 0 and less 5000 characters.")]
    public string? AddlCode { get; init; }
    [Column("date")] public DateTime? Date { get; init; }
    [Column("parent_id")] public int IdParent { get; init; }
    [Column("create_time")] public DateTime CreateTime { get; init; } = DateTime.UtcNow;
}