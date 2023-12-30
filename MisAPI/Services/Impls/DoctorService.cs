using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Exceptions;
using MisAPI.Mappers;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class DoctorService : IDoctorService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public DoctorService(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<DoctorModel> GetDoctorProfileAsync()
    {
        var doctorId = await GetDoctorGuid();
        await CheckIsRefreshTokenValid(doctorId);
        var doctor = await GetDoctorByGuidAsync(doctorId);
        return Mapper.EntityDoctorToDoctorDto(doctor);
    }


    public async Task<IActionResult> UpdateDoctorProfileAsync(DoctorEditModel doctorEditModel)
    {
        var doctorGuid = await GetDoctorGuid();

        await CheckIsEmailInUseAsync(doctorEditModel.Email, doctorGuid);
        await CheckIsRefreshTokenValid(doctorGuid);

        var doctor = await GetDoctorByGuidAsync(doctorGuid);
        var updatedDoctor = GetUpdatedDoctor(doctor, doctorEditModel);

        _db.Doctors.Update(updatedDoctor);
        await _db.SaveChangesAsync();
        return new OkResult();
    }


    private async Task<Guid> GetDoctorGuid()
    {
        var doctorId = await _jwtService.GetDoctorGuidAsync();
        if (doctorId == Guid.Empty) throw new DoctorNotFoundException("Doctor not found");
        return doctorId;
    }


    private async Task<Doctor> GetDoctorByGuidAsync(Guid doctorId)
    {
        var doctor = await _db.Doctors.SingleOrDefaultAsync(u => u.Id == doctorId);
        if (doctor == null) throw new DoctorNotFoundException("Doctor not found");
        return doctor;
    }

    private async Task CheckIsRefreshTokenValid(Guid doctorId)
    {
        var isGuidUsed = await _db.RefreshTokens.AnyAsync(token => token.DoctorId == doctorId && !token.Revoked);
        if (!isGuidUsed) throw new UnauthorizedException("Refresh token is not valid");
    }


    private static Doctor GetUpdatedDoctor(Doctor doctor, DoctorEditModel doctorEditModel)
    {
        doctor.Email = !doctorEditModel.Email.IsNullOrEmpty() ? doctorEditModel.Email : doctor.Email;
        doctor.Name = !doctorEditModel.Name.IsNullOrEmpty() ? doctorEditModel.Name : doctor.Name;
        doctor.Birthday = doctorEditModel.Birthday.ToUniversalTime();
        doctor.Gender = doctorEditModel.Gender;
        doctor.Phone = !doctorEditModel.Phone.IsNullOrEmpty() ? doctorEditModel.Phone : doctor.Phone;
        return doctor;
    }


    private async Task CheckIsEmailInUseAsync(string email, Guid doctorId)
    {
        var isEmailUsed = await _db.Doctors.AnyAsync(u => u.Email == email && u.Id != doctorId);
        if (isEmailUsed) throw new DoctorAlreadyExistsException("Email is already in use");
    }
}