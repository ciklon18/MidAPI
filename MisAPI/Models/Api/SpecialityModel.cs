namespace MisAPI.Models.Api;

public record SpecialityModel(Guid Id, string Name, DateTime CreateTime)
{
    public SpecialityModel() : this(Guid.Empty, string.Empty, DateTime.MinValue)
    {
    }
}