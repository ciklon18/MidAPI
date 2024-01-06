using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Controllers;

public class DoctorController : AuthorizeController
{
    private readonly IAuthService _authService;
    private readonly IDoctorService _doctorService;


    public DoctorController(IAuthService authService, IDoctorService doctorService)
    {
        _authService = authService;
        _doctorService = doctorService;
    }

    [HttpPost("register"), AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] DoctorRegisterModel doctorRegisterModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var newDoctor = await _authService.Register(doctorRegisterModel);
        return Ok(newDoctor);
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] DoctorLoginModel doctorLoginModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var loginResponse = await _authService.Login(doctorLoginModel);
        return Ok(loginResponse);
    }


    [HttpPost("logout"), Authorize]
    public Task<ResponseModel> Logout()
    {
        return _authService.Logout(DoctorId);
    }


    [HttpPost("refresh"), AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestModel refreshRequestModel)
    {
        var refreshResponse = await _authService.Refresh(refreshRequestModel);
        return Ok(refreshResponse);
    }


    [HttpGet("profile"), Authorize]
    public async Task<DoctorModel> GetProfile()
    {
        var doctorProfile = await _doctorService.GetDoctorProfileAsync(DoctorId);
        return doctorProfile;
    }


    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> EditProfile([FromBody] DoctorEditModel doctorEditModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _doctorService.UpdateDoctorProfileAsync(DoctorId, doctorEditModel);
        return Ok(new { message = "Profile updated successfully" });
    }
}