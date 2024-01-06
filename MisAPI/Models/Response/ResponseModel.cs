namespace MisAPI.Models.Response;

public record ResponseModel(string? Status, string? Message)
{
    public ResponseModel() : this(string.Empty, string.Empty)
    {
    }
}