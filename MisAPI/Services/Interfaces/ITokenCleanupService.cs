namespace MisAPI.Services.Interfaces;

public interface ITokenCleanupService
{
    void RemoveExpiredRefreshTokensAsync(object? state);
}