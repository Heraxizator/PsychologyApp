namespace PsychologyApp.Application.Models;

public sealed class SafetyPlanDTO
{
    public IReadOnlyList<string> WarningSigns { get; init; } = [];
    public IReadOnlyList<string> CopingStrategies { get; init; } = [];
    public IReadOnlyList<SafetyPlanContactDTO> Contacts { get; init; } = [];
    public IReadOnlyList<string> Reasons { get; init; } = [];
    public DateTime? UpdatedAt { get; init; }

    public bool IsEmpty =>
        WarningSigns.Count == 0
        && CopingStrategies.Count == 0
        && Contacts.Count == 0
        && Reasons.Count == 0;
}

public sealed class SafetyPlanContactDTO
{
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}
