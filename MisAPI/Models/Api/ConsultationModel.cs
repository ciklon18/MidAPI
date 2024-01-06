
namespace MisAPI.Models.Api;

public class ConsultationModel
{

    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid InspectionId { get; set; }
    public required SpecialityModel Speciality { get; set; }
    public ICollection<CommentModel> Comments { get; set; } = new List<CommentModel>();
}