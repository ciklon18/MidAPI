using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Consultations")]
public class Consultation
{
    [Key] [Column("id")] public Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; }
    [Column("inspection_id")] public Guid InspectionId { get; set; }
    public Inspection Inspection { get; set; } = null!;
    [Column("speciality_id")] public Guid SpecialityId { get; set; }
    public Speciality Speciality { get; set; } = null!;
    [Column("root_comment_id")] public Guid RootCommentId { get; set; }
    public Comment RootComment { get; set; } = null!;
    [Column("comments_number")] public int CommentsNumber { get; set; }
}