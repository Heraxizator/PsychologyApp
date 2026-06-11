using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class FindProblemViewModel
{
    partial void ReloadLocalizedTestContent() => ReloadTestContentAsync().FireAndForget();

    private async Task ReloadTestContentAsync()
    {
        if (string.IsNullOrWhiteSpace(_testId))
        {
            return;
        }

        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_testId);
        if (definition is null)
        {
            return;
        }

        DescriptionText = definition.Description;
        CommentText = definition.Comment;
        AlgorithmRows.Clear();
        InitAlgorithmRows(definition.Algorithm.ToList());
    }
}
