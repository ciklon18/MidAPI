using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MisAPI.Configurations;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Exceptions;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public partial class  AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext db, IJwtService jwtService, ILogger<AuthService> logger)
    {
        _db = db;
        _jwtService = jwtService;
        _logger = logger;
    }



    public async Task<RegistrationResponseModel> Register(DoctorRegisterModel doctorRegisterModel)
    {
        await IsDoctorUnique(doctorRegisterModel.Email);
        await ValidateRegisterData(doctorRegisterModel);
        await IsSpecialityExists(doctorRegisterModel.Speciality);
        var newDoctor = CreateHashDoctor(doctorRegisterModel);
        await _db.Doctors.AddAsync(newDoctor);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User successfully registered");
        return new RegistrationResponseModel { Id = newDoctor.Id };
    }

    private async Task IsSpecialityExists(Guid speciality)
    {
        var specialityExists = await _db.Specialities.AnyAsync(s => s.Id == speciality);
        if (!specialityExists) throw new SpecialityNotFoundException($"Speciality with id = {speciality} not found");
    }

    public async Task<TokenResponseModel> Login(DoctorLoginModel doctorLoginModel)
    {
        var doctor = await GetDoctorByEmailAsync(doctorLoginModel.Email);
        CheckIsValidPassword(doctorLoginModel.Password, doctor.Password);
        var existingRefreshToken = await _jwtService.GetRefreshTokenByGuidAsync(doctor.Id);
        var accessToken = _jwtService.GenerateAccessToken(doctor.Id);

        var refreshToken = existingRefreshToken ?? _jwtService.GenerateRefreshToken(doctor.Id);
        if (refreshToken != existingRefreshToken) await _jwtService.SaveRefreshTokenAsync(refreshToken, doctor.Id);
        _logger.LogInformation("User successfully log in");

        return new TokenResponseModel { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<ResponseModel> Logout(Guid doctorId)
    {
        if (doctorId == Guid.Empty) throw new DoctorNotFoundException("doctor not found");
        var refreshToken = await _jwtService.GetRefreshTokenByGuidAsync(doctorId) ??
                           throw new NullTokenException("Refresh token not found");
        await _jwtService.RevokeRefreshTokenAsync(refreshToken);
        _logger.LogInformation("User successfully logout");

        return new ResponseModel { Status = null, Message = "Logout successful" };
    }


    public async Task<RefreshResponseModel> Refresh(RefreshRequestModel refreshRequestModel)
    {
        var doctorGuid = _jwtService.GetGuidFromRefreshToken(refreshRequestModel.RefreshToken);

        await _jwtService.ValidateRefreshTokenAsync(refreshRequestModel.RefreshToken);
        var doctor = await GetDoctorByGuid(doctorGuid);
        var accessToken = _jwtService.GenerateAccessToken(doctor.Id);
        _logger.LogInformation("Access token was generated");
        return new RefreshResponseModel { AccessToken = accessToken };
    }


    private async Task IsDoctorUnique(string? doctorEmail)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(u => u.Email == doctorEmail);
        if (doctor != null) throw new DoctorAlreadyExistsException("doctor already exists");
    }


    private static void CheckIsValidPassword(string loginRequestPassword, string? doctorPassword)
    {
        if (!BCrypt.Net.BCrypt.Verify(loginRequestPassword, doctorPassword))
        {
            throw new IncorrectPasswordException("Wrong email or password");
        }
    }

    private async Task<Doctor> GetDoctorByEmailAsync(string loginRequestEmail)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(u => u.Email == loginRequestEmail);
        if (doctor == null) throw new DoctorNotFoundException("Wrong email or password");
        return doctor;
    }

    private async Task<Doctor> GetDoctorByGuid(Guid doctorId)
    {
        return await _db.Doctors.FirstOrDefaultAsync(u => u.Id == doctorId) ??
               throw new DoctorNotFoundException("doctor not found");
    }


    private static Doctor CreateHashDoctor(DoctorRegisterModel doctorRegisterModel)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(doctorRegisterModel.Password);
        return new Doctor
        {
            Name = doctorRegisterModel.Name,
            Phone = doctorRegisterModel.Phone,
            Password = passwordHash,
            Birthday = doctorRegisterModel.Birthday.ToUniversalTime(),
            Email = doctorRegisterModel.Email,
            Gender = doctorRegisterModel.Gender,
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow.ToUniversalTime(),
            SpecialityId = doctorRegisterModel.Speciality
        };
    }

    private static Task ValidateRegisterData(DoctorRegisterModel doctorRegisterModel)
    {
        if (!PhoneNumberRegex().IsMatch(doctorRegisterModel.Phone))
        {
            throw new IncorrectPhoneException(EntityConstants.IncorrectPhoneNumberError);
        }

        if (!PasswordRegex().IsMatch(doctorRegisterModel.Password))
        {
            throw new IncorrectRegisterDataException(EntityConstants.IncorrectPasswordError);
        }

        if (!EmailRegex().IsMatch(doctorRegisterModel.Email))
        {
            throw new IncorrectRegisterDataException(EntityConstants.IncorrectEmailError);
        }

        return Task.CompletedTask;
    }


    [GeneratedRegex(pattern: EntityConstants.PhoneNumberRegex)]
    private static partial Regex PhoneNumberRegex();

    [GeneratedRegex(pattern: EntityConstants.EmailRegex)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(pattern: EntityConstants.PasswordRegex)]
    private static partial Regex PasswordRegex();
}