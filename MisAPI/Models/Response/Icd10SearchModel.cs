using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record Icd10SearchModel(List<Icd10RecordModel> records, PageInfoModel pagination)
{
    public Icd10SearchModel() : this(new List<Icd10RecordModel>(), new PageInfoModel())
    {
    }
}