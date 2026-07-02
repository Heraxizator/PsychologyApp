using PsychologyApp.Application.Models;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;

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
}
