using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Habr.Common.Validators;

public static class UserValidator
{
    public static string? ValidateModelWithAnnotations<T>(this T model) where T : class
    {
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(model, new(model), validationResults, true);

        return isValid ? null : validationResults.ConvertToString();
    }

    public static string ConvertToString(this List<ValidationResult> validationResults)
    {
        StringBuilder errMessages = new();
        errMessages.AppendLine("Validation errors:");
        foreach (var error in validationResults)
        {
            errMessages.AppendLine(error.ErrorMessage);
        }

        return errMessages.ToString();
    }
}
