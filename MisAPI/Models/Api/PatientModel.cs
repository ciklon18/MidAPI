using MisAPI.Enums;

namespace MisAPI.Models.Api;

public record PatientModel(Guid Id, DateTime CreateDate, string Name, DateTime Birthday, Gender Gender)
{
    public PatientModel() : this(Guid.Empty, DateTime.Now, string.Empty, DateTime.Now, Gender.Male)
    {
    }
}