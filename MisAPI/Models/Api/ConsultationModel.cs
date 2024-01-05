namespace MisAPI.Models.Api;

public class ConsultationModel
{

    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public Guid InspectionId { get; set; }
    public SpecialityModel Speciality { get; set; }
    public CommentModel RootComment { get; set; }
    public int CommentsNumber { get; set; }
}