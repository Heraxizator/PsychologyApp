using PsychologyApp.Application.Models;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class PracticeHistoryFormatter(ITechniqueCatalogService techniqueCatalogService)
{
    public async Task<string> ResolveNameAsync(CompletionDTO completion, CancellationToken cancellationToken = default)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return completion.PageName;
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return (await techniqueCatalogService.GetAsync(techniqueId, cancellationToken)).PageName;
        }

        return string.IsNullOrWhiteSpace(completion.PageName)
            ? completion.ItemKey
            : completion.PageName;
    }

    public async Task<string> ResolveIconAsync(CompletionDTO completion, CancellationToken cancellationToken = default)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return "SelfImprovement";
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return (await techniqueCatalogService.GetAsync(techniqueId, cancellationToken)).ListIcon;
        }

        return "SelfImprovement";
    }

    public (string Text, bool HasDuration) ResolveDuration(CompletionDTO completion)
    {
        if (completion.DurationSeconds <= 0)
        {
            return (string.Empty, false);
        }

        return (AppStrings.TestResultDuration(completion.DurationSeconds), true);
    }
}
