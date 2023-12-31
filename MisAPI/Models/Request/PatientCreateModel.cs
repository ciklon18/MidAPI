using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MisAPI.Configurations;
using MisAPI.Converters;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Request;

public class PatientCreateModel
{
    [Required]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = EntityConstants.WrongSymbolInFullNameError)]
    public string Name { get; set; } = string.Empty;
    
    [DateValidatorAttribute]
    public DateTime Birthday { get; set; }
    
    [JsonConverter(typeof(GenderConverter))]
    public Gender Gender { get; set; }
}