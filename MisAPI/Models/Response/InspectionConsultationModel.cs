
using System.ComponentModel.DataAnnotations;
using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public class InspectionConsultationModel
{

    [Required(ErrorMessage = "Id is required.")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Create time is required.")]
    public DateTime CreateTime { get; set; }
    public Guid InspectionId { get; set; }
    public required SpecialityModel Speciality { get; set; }
    public InspectionCommentModel RootComment { get; set; } = null!;
    public int CommentsNumber { get; set; }
}