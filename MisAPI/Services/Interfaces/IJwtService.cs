namespace MisAPI.Services.Interfaces;

public interface IJwtService
{
    public string GenerateAccessToken(Guid doctorId);
    public string GenerateRefreshToken(Guid doctorId);
    public Task SaveRefreshTokenAsync(string refreshToken, Guid doctorId);
    public Task RevokeRefreshTokenAsync(string refreshToken);
    public Task ValidateRefreshTokenAsync(string? token);
    public Guid GetGuidFromRefreshToken(string? token);
    public Task<string?> GetRefreshTokenByGuidAsync(Guid doctorId);
    public Task<Guid> GetDoctorGuidAsync();
}