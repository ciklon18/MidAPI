using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MisAPI.Configurations;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Exceptions;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class JwtService : IJwtService
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly Tokens _tokens;
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtService(JwtSecurityTokenHandler tokenHandler, Tokens tokens, ApplicationDbContext db,
        IHttpContextAccessor httpContextAccessor)
    {
        _tokenHandler = tokenHandler;
        _tokens = tokens;
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }


    public string GenerateAccessToken(Guid doctorId)
    {
        return GenerateToken(doctorId: doctorId, key: _tokens.AccessTokenKey,
            expirationTime: _tokens.AccessTokenExpiration.TotalMinutes);
    }

    public string GenerateRefreshToken(Guid doctorId)
    {
        return GenerateToken(doctorId: doctorId, key: _tokens.RefreshTokenKey,
            expirationTime: _tokens.RefreshTokenExpiration.TotalMinutes);
    }


    public async Task SaveRefreshTokenAsync(string refreshToken, Guid doctorId)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_tokens.RefreshTokenExpiration.TotalMinutes);
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            DoctorId = doctorId,
            Expires = expiration
        });
        await _db.SaveChangesAsync();
    }

    public Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = _db.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
        if (token == null) return Task.CompletedTask;
        token.Revoked = true;
        _db.RefreshTokens.Update(token);
        return _db.SaveChangesAsync();
    }

    public async Task ValidateRefreshTokenAsync(string? token)
    {
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (refreshToken == null) throw new NullTokenException("Refresh token is null");
        CheckTokenNotRevokedAndNotExpired(refreshToken);
    }

    public async Task<Guid> GetDoctorGuidAsync()
    {
        var stringDoctorId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var doctorGuid = Guid.Parse(stringDoctorId ?? string.Empty);
        if (doctorGuid == Guid.Empty) throw new DoctorNotFoundException("doctor not found");
        await CheckIsRefreshTokenValidAsync(doctorGuid);
        var doctor = await _db.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == doctorGuid);
        if (doctor == null) throw new DoctorNotFoundException("doctor not found");
        return doctor.Id;
    }


    public Guid GetGuidFromRefreshToken(string? token)
    {
        var refreshToken = _db.RefreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null) throw new NullTokenException("Refresh token is null");

        return refreshToken.DoctorId;
    }


    public async Task<string?> GetRefreshTokenByGuidAsync(Guid doctorId)
    {
        var refreshToken =
            await _db.RefreshTokens.FirstOrDefaultAsync(t => t.DoctorId == doctorId && !t.Revoked && t.Expires > DateTime.UtcNow);
        return refreshToken?.Token;
    }

    public async Task CheckIsRefreshTokenValidAsync(Guid doctorId)
    {
        var isGuidUsed = await _db.RefreshTokens.AnyAsync(token => token.DoctorId == doctorId);
        if (!isGuidUsed) throw new UnauthorizedException("Refresh token is not valid");
    }


    private string GenerateToken(Guid doctorId, SecurityKey? key, double expirationTime)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, doctorId.ToString()),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationTime),
            SigningCredentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }


    private static void CheckTokenNotRevokedAndNotExpired(RefreshToken? refreshToken)
    {
        switch (refreshToken)
        {
            case { Revoked: true }:
                throw new InvalidTokenException("Invalid or revoked refresh token");
            case { IsExpired: true }:
                throw new ExpiredRefreshTokenException(
                    $"Refresh token is expired. Expiration date: {refreshToken.Expires}");
        }
    }
}