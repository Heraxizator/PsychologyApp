using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.UserProgress;
using System.Text.Json;
using PsychologyApp.Application.Serialization;

namespace PsychologyApp.Application.Tests;

public sealed record LuscherColorSelection(string Code, string? DisplayName);

public interface ILuscherResultService
{
    Task SaveStandardAsync(
        IUserProgressService progress,
        string summary,
        int coValue,
        double bkValue,
        IReadOnlyList<LuscherColorSelection> firstPassColors,
        IReadOnlyList<LuscherColorSelection> secondPassColors,
        CancellationToken cancellationToken = default);

    Task SaveBriefAsync(
        IUserProgressService progress,
        string summary,
        string? firstName,
        string? secondName,
        string? firstCode,
        string? secondCode,
        string? firstResult,
        string? secondResult,
        CancellationToken cancellationToken = default);
}

public sealed class LuscherResultService : ILuscherResultService
{
    public Task SaveStandardAsync(
        IUserProgressService progress,
        string summary,
        int coValue,
        double bkValue,
        IReadOnlyList<LuscherColorSelection> firstPassColors,
        IReadOnlyList<LuscherColorSelection> secondPassColors,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        LuscherStandardResultDetail detail = new()
        {
            Co = coValue,
            Bk = Math.Round(bkValue, 2),
            FirstPassColors = MapColors(firstPassColors),
            Colors = MapColors(secondPassColors)
        };

        string detailJson = JsonSerializer.Serialize(detail, TestJsonSerializerContext.Default.LuscherStandardResultDetail);

        return progress.SaveTestResultAsync(TestIds.LuscherStandard, coValue, summary, detailJson, cancellationToken);
    }

    public Task SaveBriefAsync(
        IUserProgressService progress,
        string summary,
        string? firstName,
        string? secondName,
        string? firstCode,
        string? secondCode,
        string? firstResult,
        string? secondResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        LuscherBriefResultDetail detail = new()
        {
            First = new LuscherBriefColorDetail { Code = firstCode, Name = firstName, Text = firstResult },
            Second = new LuscherBriefColorDetail { Code = secondCode, Name = secondName, Text = secondResult }
        };

        string detailJson = JsonSerializer.Serialize(detail, TestJsonSerializerContext.Default.LuscherBriefResultDetail);

        return progress.SaveTestResultAsync(TestIds.LuscherBrief, null, summary, detailJson, cancellationToken);
    }

    private static List<LuscherStandardColorDetail> MapColors(IReadOnlyList<LuscherColorSelection> colors) =>
        colors
            .Select(item => new LuscherStandardColorDetail
            {
                Code = item.Code,
                Name = item.DisplayName
            })
            .ToList();
}
