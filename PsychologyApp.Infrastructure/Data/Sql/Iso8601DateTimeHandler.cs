using Dapper;
using System.Data;
using System.Globalization;

namespace PsychologyApp.Infrastructure.Data.Sql;

internal sealed class Iso8601DateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public static void Register() => SqlMapper.AddTypeHandler(new Iso8601DateTimeHandler());

    public override void SetValue(IDbDataParameter parameter, DateTime value) =>
        parameter.Value = value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    public override DateTime Parse(object value) =>
        value switch
        {
            DateTime dateTime => dateTime,
            long ticks => new DateTime(ticks, DateTimeKind.Utc),
            string text => text.EndsWith('Z')
                ? DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                : DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            _ => throw new DataException($"Cannot convert {value?.GetType().FullName ?? "null"} to DateTime.")
        };
}
