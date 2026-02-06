using System.Text.RegularExpressions;
using EwidencjaSprzetuOOP.Domain.Exceptions;

namespace EwidencjaSprzetuOOP.Domain.Validation;

public static class Validators
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static void Required(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"Pole '{fieldName}' jest wymagane.");
    }

    public static void MaxLen(string? value, int max, string fieldName)
    {
        if (value is not null && value.Length > max)
            throw new ValidationException($"Pole '{fieldName}' może mieć maksymalnie {max} znaków.");
    }

    public static void Email(string? value, string fieldName)
    {
        Required(value, fieldName);
        if (!EmailRegex.IsMatch(value!))
            throw new ValidationException($"Pole '{fieldName}' ma niepoprawny format email.");
    }

    public static void PositiveInt(int value, string fieldName)
    {
        if (value <= 0)
            throw new ValidationException($"Pole '{fieldName}' musi być > 0.");
    }
}
