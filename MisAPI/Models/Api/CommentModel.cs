namespace MisAPI.Models.Api;

public class CommentModel
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid? ParentId { get; set; }
    public string? Content { get; set; } = null!;
    public DoctorModel Author { get; set; } = null!;
    public DateTime? ModifyTime { get; set; }
}