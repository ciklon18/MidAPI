using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MisAPI.Controllers;

[Authorize]
public abstract class AuthorizeController : BaseController
{
    protected Guid DoctorId => Guid.Parse(User.FindFirstValue("UserID") ?? string.Empty);
}