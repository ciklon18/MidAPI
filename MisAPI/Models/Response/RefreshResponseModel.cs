namespace MisAPI.Models.Response;

public record RefreshResponseModel(string AccessToken)
{
    public RefreshResponseModel() : this(string.Empty)
    {
    }
}