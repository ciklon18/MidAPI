namespace MisAPI.Models.Request;

public record RefreshRequestModel(string RefreshToken)
{
    public RefreshRequestModel() : this(string.Empty)
    {
    }
}