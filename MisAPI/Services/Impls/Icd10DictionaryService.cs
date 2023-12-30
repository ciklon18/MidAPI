using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class Icd10DictionaryService : IIcd10DictionaryService
{

    public Task<SpecialtiesPagedListModel> GetSpecialtiesAsync(string? name, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<Icd10SearchModel> GetIcd10DiagnosesAsync(string? request, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<Icd10RootsResponseModel> GetIcd10RootsAsync()
    {
        throw new NotImplementedException();
    }
}