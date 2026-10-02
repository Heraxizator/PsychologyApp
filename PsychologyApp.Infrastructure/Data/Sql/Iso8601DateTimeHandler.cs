using Dapper;
using System.Data;
using System.Globalization;

namespace PsychologyApp.Infrastructure.Data.Sql;

internal sealed class Iso8601DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public static void Register() => SqlMapper.AddTypeHandler(new Iso8601DateTimeHandler());

    public override void SetValue(IDbDataParameter parameter, DateTime value) =>
        parameter.Value = SqliteTime.ToIso(value);

    // Always UTC-kind, whether or not the stored text carries a "Z" or an offset (AssumeUniversal alone would return local time).
    public override DateTime Parse(object value) =>
        value switch
        {
            DateTime dateTime => dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime(),
            long ticks => new DateTime(ticks, DateTimeKind.Utc),
            string text => DateTime.Parse(
                text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
            _ => throw new DataException($"Cannot convert {value?.GetType().FullName ?? "null"} to DateTime.")
        };
}
