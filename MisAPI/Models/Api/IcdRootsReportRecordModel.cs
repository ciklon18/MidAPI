using MisAPI.Enums;

namespace MisAPI.Models.Api;

public class IcdRootsReportRecordModel
{

    public string PatientName { get; set; } = null!;
    public DateTime PatientBirthdate { get; set; }
    public Gender Gender { get; set; }
    public Dictionary<string, int> VisitsByRoot { get; set; } = null!;
    
    
}