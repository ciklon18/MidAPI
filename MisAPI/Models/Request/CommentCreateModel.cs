using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MisAPI.Models.Request;

public class CommentCreateModel
{
    public CommentCreateModel(string content, Guid? parentId)
    {
        Content = content;
        ParentId = parentId ?? Guid.Empty;
    }

    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment length must be between 1 and 1000 characters")]
    public string Content { get; set; }
    
    [JsonConverter(typeof(GuidConverter))]
    public Guid ParentId { get; set; }
}