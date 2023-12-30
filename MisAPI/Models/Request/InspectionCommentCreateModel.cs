using System.ComponentModel.DataAnnotations;

namespace MisAPI.Models.Request;

public class InspectionCommentCreateModel
{
    public InspectionCommentCreateModel(string content)
    {
        Content = content;
    }

    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment length must be between 1 and 1000 characters")]
    public string Content { get; set; }
}