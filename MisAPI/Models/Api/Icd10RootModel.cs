namespace MisAPI.Models.Api;

public class Icd10RootModel
{
    public Icd10RootModel(Guid id, DateTime createTime, string code, string name)
    {
        Id = id;
        CreateTime = createTime;
        Code = code;
        Name = name;
    }

    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}