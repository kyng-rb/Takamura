namespace Takamura.Application.Database;

/// <summary>Column sizes from <see cref="DatabaseContext"/> conventions, shared with input validation.</summary>
public static class FieldLimits
{
    /// <summary>Every <c>string</c> column is <c>varchar(TextMaxLength)</c>.</summary>
    public const int TextMaxLength = 100;

    /// <summary>Enums are stored as <c>varchar(EnumMaxLength)</c>.</summary>
    public const int EnumMaxLength = 20;
}
