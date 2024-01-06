namespace MisAPI.Models.Api;

public class InspectionCommentModel
{
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid? ParentId { get; set; }
    public string? Content { get; set; }
    public DoctorModel Author { get; set; } = null!;
    public DateTime ModifyTime { get; set; }
}