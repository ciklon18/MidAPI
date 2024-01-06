using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IDoctorService
{
    public Task<DoctorModel> GetDoctorProfileAsync(Guid doctorId);
    public Task<IActionResult> UpdateDoctorProfileAsync(Guid doctorId, DoctorEditModel userEditModel);
}