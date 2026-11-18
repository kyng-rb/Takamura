namespace Takamura.Application.Database.Entities.PeriodBudget;

public static class BudgetTypeParser
{
    public const string AllowedValues = "Income, Outcome, Saving";

    /// <summary>
    /// Case-insensitive match on the enum names only. Numbers ("1") and undefined values are rejected,
    /// which <see cref="Enum.TryParse{TEnum}(string, bool, out TEnum)"/> alone would accept.
    /// </summary>
    public static bool TryParse(string? value, out BudgetType type)
    {
        type = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();
        if (!char.IsLetter(trimmed[0]))
            return false;

        return Enum.TryParse(trimmed, ignoreCase: true, out type) && Enum.IsDefined(type);
    }

    /// <summary>For values already validated with <see cref="TryParse"/>.</summary>
    public static BudgetType Parse(string value)
        => TryParse(value, out var type)
            ? type
            : throw new ArgumentException($"Invalid budget type '{value}'. Allowed values: {AllowedValues}", nameof(value));
}
