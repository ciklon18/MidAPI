using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MisAPI.Entities;

[Table("Comments")]
public class Comment
{
    [Key] [Column("id")] public Guid Id { get; set; }
    [Column("create_time")] public DateTime CreateTime { get; set; }
    [Column("parent_id")] public Guid? ParentId { get; set; }
    [Column("content")] public string? Content { get; set; }
    [Column("author_id")] public Guid AuthorId { get; set; }
    public Doctor Author { get; set; } = null!;
    [Column("modify_time")] public DateTime? ModifyTime { get; set; }

    [Column("consultation_id")] public Guid ConsultationId { get; set; }
    public Consultation Consultation { get; set; } = null!;
    public ICollection<Comment>? Children { get; set; } = new List<Comment>();
}