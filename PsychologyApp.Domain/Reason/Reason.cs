using PsychologyApp.Domain.Base;

namespace PsychologyApp.Domain.Entities;

public class Reason : Entity
{
    public long ReasonId { get; private init; }
    public string Title { get; private set; } = default!;
    public string Subtitle { get; private set; } = default!;
    public string Solution { get; private set; } = default!;

    public static Reason Create(string title, string subtitle, string solution)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(subtitle);
        ArgumentException.ThrowIfNullOrWhiteSpace(solution);

        return new Reason
        {
            Title = title,
            Subtitle = subtitle,
            Solution = solution
        };
    }
}
