using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Models.Practice;

/// <param name="Suggestions">Typical short answers offered as chips: one tap adds the words instead of typing them.</param>
/// <param name="Role">What the field is about, so the screen can offer the right help (last problem, emotion palette, kind phrases).</param>
public sealed record TechniqueEntrySeed(
    string Title,
    string Placeholder = "",
    EntryFieldKind Kind = EntryFieldKind.Text,
    IReadOnlyList<string>? Suggestions = null,
    EntryFieldRole Role = EntryFieldRole.None);
