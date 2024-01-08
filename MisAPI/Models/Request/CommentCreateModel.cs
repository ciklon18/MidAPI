using System.ComponentModel.DataAnnotations;

namespace MisAPI.Models.Request;

public class CommentCreateModel
{

    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment length must be between 1 and 1000 characters")]
    public required string Content { get; set; }
    public Guid ParentId { get; set; }
}