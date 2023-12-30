using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MisAPI.Configurations;
using MisAPI.Converters;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Request;

public class DoctorEditModel
{
    public DoctorEditModel(string email, string name, DateTime birthday, Gender gender, string phone)
    {
        Email = email;
        Name = name;
        Birthday = birthday;
        Gender = gender;
        Phone = phone;
    }

    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = EntityConstants.ShortOrLongEmailError)]
    [RegularExpression(pattern: EntityConstants.EmailRegex,
        ErrorMessage = EntityConstants.IncorrectEmailError)]
    public string Email { get; set; }

    [StringLength(maximumLength: 200, MinimumLength = 1)]
    public string Name { get; set; }
    
    [JsonConverter(typeof(JsonDateTimeConverter))]
    [DateValidator(ErrorMessage = EntityConstants.IncorrectDateError)]
    public DateTime Birthday { get; set; }
    
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    
    [StringLength(maximumLength: 12)]
    [RegularExpression(pattern: EntityConstants.PhoneNumberRegex,
        ErrorMessage = EntityConstants.IncorrectPhoneNumberError)]
    public string Phone { get; set; }
}