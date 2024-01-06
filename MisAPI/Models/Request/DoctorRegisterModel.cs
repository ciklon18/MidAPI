using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MisAPI.Configurations;
using MisAPI.Converters;
using MisAPI.Enums;
using MisAPI.Validator;

namespace MisAPI.Models.Request;

public class DoctorRegisterModel
{
    [StringLength(maximumLength: 200, MinimumLength = 1)]
    public string Name { get; set; }

    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = EntityConstants.ShortPasswordError)]
    [RegularExpression(pattern: EntityConstants.PasswordRegex,
        ErrorMessage = EntityConstants.IncorrectPasswordError)]
    public string Password { get; set; }

    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = EntityConstants.ShortOrLongEmailError)]
    [RegularExpression(pattern: EntityConstants.EmailRegex,
        ErrorMessage = EntityConstants.IncorrectEmailError)]
    public string Email { get; set; }

    [JsonConverter(typeof(JsonDateTimeConverter))]
    [DateValidator(ErrorMessage = EntityConstants.IncorrectDateError)]
    public DateTime Birthday { get; set; }

    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [StringLength(maximumLength: 12)]
    [RegularExpression(pattern: EntityConstants.PhoneNumberRegex,
        ErrorMessage = EntityConstants.IncorrectPhoneNumberError)]
    public string Phone { get; set; }

    public Guid Speciality { get; set; }

    public DoctorRegisterModel(string name, string password, string email, DateTime birthday, Gender gender, string phone, Guid speciality)
    {
        Name = name;
        Password = password;
        Email = email;
        Birthday = birthday;
        Gender = gender;
        Phone = phone;
        Speciality = speciality;
    }
}
