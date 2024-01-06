using System.ComponentModel.DataAnnotations;

namespace MisAPI.Models.Request;

public class InspectionCommentCreateModel
{
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Comment length must be between 1 and 5000.")]
    [Required(ErrorMessage = "Content is required.")]
    public required string Content { get; set; }
}