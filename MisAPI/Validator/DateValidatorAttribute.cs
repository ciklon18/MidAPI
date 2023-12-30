using System.ComponentModel.DataAnnotations;

namespace MisAPI.Validator;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateValidatorAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {

        if (DateTime.TryParse(value?.ToString(), out var parsedDateTime))
        {
            return parsedDateTime < DateTime.Now;
        }
        
        return false;
    }

}