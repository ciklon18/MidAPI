namespace MisAPI.Models.Api;

public record PageInfoModel(int Size, int Count, int CurrentPage)
{
    public PageInfoModel() : this(0, 0, 0)
    {
    }
}
