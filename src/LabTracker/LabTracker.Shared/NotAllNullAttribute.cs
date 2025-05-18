using System.ComponentModel.DataAnnotations;

namespace Shared;

[AttributeUsage(AttributeTargets.Class)]
public class NotAllNullAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is null)
            return new ValidationResult("Object cannot be null.");

        var allPropertiesNull = value.GetType()
            .GetProperties()
            .All(prop => prop.GetValue(value) is null);

        return allPropertiesNull
            ? new ValidationResult("At least one property cannot be null.")
            : ValidationResult.Success;
    }
}