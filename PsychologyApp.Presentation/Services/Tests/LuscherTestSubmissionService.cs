using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using System.Text.Json;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed class LuscherTestSubmissionService
{
    public Task SaveStandardAsync(
        IUserProgressService? progress,
        int coValue,
        double bkValue,
        IEnumerable<(ColourValue Colour, ColourMeaning Meaning)> selectedColors,
        CancellationToken cancellationToken = default)
    {
        if (progress is null)
        {
            return Task.CompletedTask;
        }

        string summary = $"{AppStrings.TestsCoLabel}: {coValue}; {AppStrings.TestsBkLabel}: {Math.Round(bkValue, 2)}";
        string detailJson = JsonSerializer.Serialize(new
        {
            co = coValue,
            bk = Math.Round(bkValue, 2),
            colors = selectedColors.Select(item => new
            {
                code = item.Colour.Code,
                name = ColourStrings.GetColorName(item.Colour)
            }).ToList()
        });

        return progress.SaveTestResultAsync(TestIds.LuscherStandard, coValue, summary, detailJson, cancellationToken);
    }

    public Task SaveBriefAsync(
        IUserProgressService? progress,
        string? firstName,
        string? secondName,
        string? firstResult,
        string? secondResult,
        CancellationToken cancellationToken = default)
    {
        if (progress is null)
        {
            return Task.CompletedTask;
        }

        string summary = $"{firstName} / {secondName}";
        string detailJson = JsonSerializer.Serialize(new
        {
            first = new { name = firstName, text = firstResult },
            second = new { name = secondName, text = secondResult }
        });

        return progress.SaveTestResultAsync(TestIds.LuscherBrief, null, summary, detailJson, cancellationToken);
    }
}
