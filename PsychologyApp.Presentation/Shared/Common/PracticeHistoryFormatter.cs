using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.Shared.Common;

public sealed class PracticeHistoryFormatter(TechniqueCatalogGateway techniqueCatalog)
{
    public string ResolveName(CompletionDTO completion)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return completion.PageName;
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return techniqueCatalog.Get(techniqueId).PageName;
        }

        return string.IsNullOrWhiteSpace(completion.PageName)
            ? completion.ItemKey
            : completion.PageName;
    }
}
