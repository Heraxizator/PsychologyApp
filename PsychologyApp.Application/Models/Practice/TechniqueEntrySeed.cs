using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Models.Practice;

public sealed record TechniqueEntrySeed(
    string Title,
    string Placeholder = "",
    EntryFieldKind Kind = EntryFieldKind.Text);
