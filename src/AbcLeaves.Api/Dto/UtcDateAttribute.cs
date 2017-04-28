using System;
using System.ComponentModel.DataAnnotations;

namespace AbcLeaves.Api
{
    public class UtcDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var member = validationContext.MemberName;
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (!(value is DateTime))
            {
                return new ValidationResult(
                    $"The {member} field is supposed to be of datetime type.");
            }
            var dateTime = (DateTime)value;
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                return new ValidationResult(
                    $"The {member} field has to be a UTC datetime.");
            }
            return ValidationResult.Success;
        }
    }
}
