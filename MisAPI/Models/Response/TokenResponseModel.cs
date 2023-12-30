namespace MisAPI.Models.Response;

public record TokenResponseModel(string AccessToken, string RefreshToken)
{
    public TokenResponseModel() : this(string.Empty, string.Empty)
    {
    }
}