using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Takamura.Application.Database;

/// <summary>
/// Every <see cref="DateTime"/> is stored in UTC. SQL Server keeps no kind, so values read back
/// are marked <see cref="DateTimeKind.Utc"/> instead of <see cref="DateTimeKind.Unspecified"/>.
/// </summary>
public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(),
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
