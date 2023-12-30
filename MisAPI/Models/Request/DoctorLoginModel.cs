using System.ComponentModel.DataAnnotations;
using MisAPI.Configurations;

namespace MisAPI.Models.Request;

public class DoctorLoginModel
{
    public DoctorLoginModel(string email, string password)
    {
        Email = email;
        Password = password;
    }

    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = EntityConstants.ShortOrLongEmailError)]
    [RegularExpression(pattern: EntityConstants.EmailRegex,
        ErrorMessage = EntityConstants.IncorrectEmailError)]
    public string Email { get; set; }

    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = EntityConstants.ShortPasswordError)]
    [RegularExpression(pattern: EntityConstants.PasswordRegex,
        ErrorMessage = EntityConstants.IncorrectPasswordError)]
    public string Password { get; set; }
}