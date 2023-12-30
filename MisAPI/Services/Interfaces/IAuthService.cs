using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IAuthService
{
    Task<RegistrationResponseModel> Register(DoctorRegisterModel doctorRegisterModel);
    Task<TokenResponseModel> Login(DoctorLoginModel doctorLoginModel);
    Task<ResponseModel> Logout();
    Task<RefreshResponseModel> Refresh(RefreshRequestModel refreshRequestModel);
}