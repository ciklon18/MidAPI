namespace MisAPI.Models.Response;

public record ConsultationModel(Guid Id, DateTime CreateTime, Guid InspectionId
    // , SpecialityModel speciality, IEnumerable<CommentModel>? comments
    )
{
    public ConsultationModel() : this(Guid.Empty, DateTime.MinValue, Guid.Empty
        // , new SpecialityModel(), new List<CommentModel>()
        )
    {
    }
}
