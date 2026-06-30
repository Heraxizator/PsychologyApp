using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.UserProgress;
using System.Text.Json;

namespace PsychologyApp.Application.Tests;

public sealed record LuscherColorSelection(string Code, string? DisplayName);

public interface ILuscherResultService
{
    Task SaveStandardAsync(
        IUserProgressService progress,
        string summary,
        int coValue,
        double bkValue,
        IReadOnlyList<LuscherColorSelection> selectedColors,
        CancellationToken cancellationToken = default);

    Task SaveBriefAsync(
        IUserProgressService progress,
        string summary,
        string? firstName,
        string? secondName,
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
        IReadOnlyList<LuscherColorSelection> selectedColors,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        string detailJson = JsonSerializer.Serialize(new
        {
            co = coValue,
            bk = Math.Round(bkValue, 2),
            colors = selectedColors.Select(item => new
            {
                code = item.Code,
                name = item.DisplayName
            }).ToList()
        });

        return progress.SaveTestResultAsync(TestIds.LuscherStandard, coValue, summary, detailJson, cancellationToken);
    }

    public Task SaveBriefAsync(
        IUserProgressService progress,
        string summary,
        string? firstName,
        string? secondName,
        string? firstResult,
        string? secondResult,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        string detailJson = JsonSerializer.Serialize(new
        {
            first = new { name = firstName, text = firstResult },
            second = new { name = secondName, text = secondResult }
        });

        return progress.SaveTestResultAsync(TestIds.LuscherBrief, null, summary, detailJson, cancellationToken);
    }
}
