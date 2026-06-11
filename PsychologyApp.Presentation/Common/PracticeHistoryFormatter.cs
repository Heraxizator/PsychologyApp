using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Common;

public static class PracticeHistoryFormatter
{
    public static string ResolveName(CompletionDTO completion)
    {
        if (completion.ItemKey.StartsWith("custom_", StringComparison.Ordinal))
        {
            return completion.PageName;
        }

        if (Enum.TryParse(completion.ItemKey, out TechniqueId techniqueId))
        {
            return TechniqueCatalog.Get(techniqueId).PageName;
        }

        return string.IsNullOrWhiteSpace(completion.PageName)
            ? completion.ItemKey
            : completion.PageName;
    }
}
