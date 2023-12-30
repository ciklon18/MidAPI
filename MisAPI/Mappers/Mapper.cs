using MisAPI.Entities;
using MisAPI.Models.Api;

namespace MisAPI.Mappers;

public static class Mapper
{
    public static DoctorModel EntityDoctorToDoctorDto(Doctor doctor)
    {
        return new DoctorModel
        {
            Id = doctor.Id,
            CreateTime = doctor.CreateTime.ToUniversalTime(),
            Name = doctor.Name,
            Birthday = doctor.Birthday.ToUniversalTime(),
            Gender = doctor.Gender,
            Email = doctor.Email,
            Phone = doctor.Phone ?? string.Empty
        };
    }
}