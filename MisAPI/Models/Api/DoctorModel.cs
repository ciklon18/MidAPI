using MisAPI.Enums;

namespace MisAPI.Models.Api;

public record DoctorModel(
    Guid Id,
    DateTime CreateTime,
    string Name,
    DateTime Birthday,
    Gender Gender,
    string Email,
    string Phone)
{
    public DoctorModel() : this(Guid.Empty, DateTime.Now, string.Empty, DateTime.Now, Gender.Male, string.Empty,
        string.Empty)
    {
        
    }
}