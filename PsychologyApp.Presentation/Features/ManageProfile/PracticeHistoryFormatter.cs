using PsychologyApp.Application.Models;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class PracticeHistoryFormatter(ITechniqueCatalogService techniqueCatalogService)
{
    public string ResolveName(CompletionDTO completion)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return completion.PageName;
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return techniqueCatalogService.Get(techniqueId).PageName;
        }

        return string.IsNullOrWhiteSpace(completion.PageName)
            ? completion.ItemKey
            : completion.PageName;
    }

    public string ResolveIcon(CompletionDTO completion)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return "SelfImprovement";
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return techniqueCatalogService.Get(techniqueId).ListIcon;
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
