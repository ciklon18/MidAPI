using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record Icd10RootsResponseModel(List<Icd10RecordModel> Roots)
{
    public Icd10RootsResponseModel() : this(new List<Icd10RecordModel>())
    {
    }
}