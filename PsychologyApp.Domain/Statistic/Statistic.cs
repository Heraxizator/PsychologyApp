using PsychologyApp.Domain.Base;

namespace PsychologyApp.Domain.Entities;

public class Statistic : Entity
{
    public long StatisticId { get; private init; }
    public string ModuleName { get; private set; } = default!;
    public string PageName { get; private set; } = default!;
    public DateTime DateTime { get; private set; }
    public long SecondsDuration { get; private set; }

    public static Statistic Create(string moduleName, string pageName, DateTime dateTime, long secondsDuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleName);
        ArgumentException.ThrowIfNullOrWhiteSpace(pageName);

        if (secondsDuration < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(secondsDuration), "Duration cannot be negative.");
        }

        return new Statistic
        {
            ModuleName = moduleName,
            PageName = pageName,
            DateTime = dateTime,
            SecondsDuration = secondsDuration
        };
    }

    public static Statistic CreateWithDuration(string moduleName, string pageName, DateTime startedAt, DateTime endedAt)
    {
        long seconds = (long)Math.Max(0, (endedAt - startedAt).TotalSeconds);
        return Create(moduleName, pageName, endedAt, seconds);
    }
}
