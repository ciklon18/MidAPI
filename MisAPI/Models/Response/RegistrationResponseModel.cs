namespace MisAPI.Models.Response;

public record RegistrationResponseModel(Guid Id)
{
    public RegistrationResponseModel() : this(Guid.Empty)
    {
    }
}