namespace MisAPI.Models.Request;

public class ConsultationCreateModel
{
    public Guid SpecialityId { get; set; }
    public InspectionCommentCreateModel Comment { get; set; } = null!;
}