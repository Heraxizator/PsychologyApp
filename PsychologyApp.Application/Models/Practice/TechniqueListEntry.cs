using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Models.Practice;

public sealed record TechniqueListEntry(
    TechniqueId TechniqueId,
    string Number,
    string Date,
    string Title,
    string Subtitle,
    string Theme,
    string Author,
    int DurationMinutes,
    string Icon);
