namespace Takamura.Application.Database;

public static class UniqueIndex
{
    /// <summary>
    /// Filter for unique indexes so soft-deleted rows don't block reusing a name.
    /// SQL Server's default collation is case-insensitive, so "Gasolina" and "gasolina" collide.
    /// </summary>
    public const string NotDeletedFilter = "[Status] <> 'Deleted'";
}
