using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class LuscherColorDisplayFactory
{
    public static LuscherColorDisplayItem FromColour(ColourValue colour, string name) =>
        new(colour.Code, name, LuscherColorMapper.ToMauiColor(colour));

    public static LuscherColorDisplayItem FromStandardColor(LuscherStandardColorDetail color)
    {
        string code = string.IsNullOrWhiteSpace(color.Code) ? "#888888" : color.Code;
        return new LuscherColorDisplayItem(code, color.Name ?? code, Color.FromArgb(code));
    }

    public static LuscherColorDisplayItem FromBriefColor(LuscherBriefColorDetail? color)
    {
        if (color is null)
        {
            return new LuscherColorDisplayItem("#888888", string.Empty, Colors.Gray);
        }

        string code = string.IsNullOrWhiteSpace(color.Code) ? "#888888" : color.Code;
        return new LuscherColorDisplayItem(code, color.Name ?? code, Color.FromArgb(code));
    }

    public static IReadOnlyList<LuscherColorDisplayItem> FromStandardPass(IReadOnlyList<LuscherStandardColorDetail>? colors) =>
        colors is null || colors.Count == 0
            ? []
            : colors.Select(FromStandardColor).ToList();

    public static IReadOnlyList<LuscherColorDisplayItem> FromSelections(
        IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> selections) =>
        selections
            .Select(item => FromColour(item.Colour, ColourStrings.GetColorName(item.Colour)))
            .ToList();
}
