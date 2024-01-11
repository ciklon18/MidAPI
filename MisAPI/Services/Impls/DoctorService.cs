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
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(ApplicationDbContext db, ILogger<DoctorService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DoctorModel> GetDoctorProfileAsync(Guid doctorId)
    {
        await CheckIsRefreshTokenValid(doctorId);
        var doctor = await GetDoctorByGuidAsync(doctorId);
        _logger.LogInformation("Doctor with id = {id} was found", doctorId);
        return Mapper.MapEntityDoctorToDoctorDto(doctor);
    }


    public async Task<IActionResult> UpdateDoctorProfileAsync(Guid doctorId, DoctorEditModel doctorEditModel)
    {
        await CheckIsEmailInUseAsync(doctorEditModel.Email, doctorId);
        await CheckIsRefreshTokenValid(doctorId);

        var doctor = await GetDoctorByGuidAsync(doctorId);
        var updatedDoctor = GetUpdatedDoctor(doctor, doctorEditModel);

        _db.Doctors.Update(updatedDoctor);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Doctor with id = {id} was found and updated", doctorId);

        return new OkResult();
    }
    

    private async Task<Doctor> GetDoctorByGuidAsync(Guid doctorId)
    {
        return await _db.Doctors.SingleOrDefaultAsync(u => u.Id == doctorId) ?? throw new DoctorNotFoundException("Doctor not found");
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