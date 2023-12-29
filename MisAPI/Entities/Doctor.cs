using MisAPI.Configurations;
using MisAPI.Converters;
using MisAPI.Enums;

namespace MisAPI.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("Doctors")]
public class Doctor
{
    [Key] [Column("id")] public Guid Id { get; init; }

    [Column("full_name")]
    [StringLength(maximumLength: 200)]

    [RegularExpression(pattern: EntityConstants.FullNameRegex,
        ErrorMessage = EntityConstants.WrongSymbolInFullNameError)]
    public required string Name { get; set; }

    [Column("birth_date")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime Birthday { get; set; }

    [Column("email")]
    [StringLength(maximumLength: 100)]
    [EmailAddress(ErrorMessage = EntityConstants.IncorrectEmailError)]
    public required string Email { get; set; }


    [Column("phone")]
    [StringLength(maximumLength: 12)]
    [RegularExpression(pattern: EntityConstants.PhoneNumberRegex,
        ErrorMessage = EntityConstants.IncorrectPhoneNumberError)]
    public string? Phone { get; set; }

    [Column("gender")]
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [Column("password")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = EntityConstants.ShortPasswordError)]
    public string? Password { get; set; }

    [Required]
    [Column("created_at")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime CreateTime { get; set; }
}