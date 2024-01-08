using MisAPI.Models.Api;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IIcd10DictionaryService
{
    Task<SpecialtiesPagedListModel> GetSpecialtiesAsync(string? name, int page, int size);
    Task<Icd10SearchModel> GetIcd10DiagnosesAsync(string? request, int page, int size);
    Task<Icd10RootsResponseModel> GetIcd10RootsAsync();
    Task<ExtendedDiagnosisModel> GetIcd10DiagnosisAsync(Guid icdDiagnosisId);
    Task CheckAreIcdRootsExist(ICollection<Guid>? icdRoots);
    Task<ICollection<Icd10RootModel>> GetRootsByIcdList(ICollection<Guid>? icdRoots);
    Task<ICollection<Icd10RootModel>> GetRootsWithoutIcdList();
}