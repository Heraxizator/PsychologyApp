namespace PsychologyApp.Infrastructure.Data.Sql;

public sealed class EntitySqlMap
{
    public required string Table { get; init; }
    public required string KeyColumn { get; init; }
    public required string InsertSql { get; init; }
    public required string UpdateSql { get; init; }
    public required string DeleteSql { get; init; }
    public required string SelectAllSql { get; init; }
    public required string SelectByKeySql { get; init; }
}
