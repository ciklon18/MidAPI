namespace MisAPI.Models.Api;

public record Icd10RecordModel(Guid Id, DateTime CreateTime, string? Code, string Name)
{
    public Icd10RecordModel() : this(Guid.Empty, DateTime.Now, string.Empty, string.Empty)
    {
    }
}