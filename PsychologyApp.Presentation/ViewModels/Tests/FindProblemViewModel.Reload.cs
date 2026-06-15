using PsychologyApp.Presentation.Common;
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

        FindProblemContentSnapshot? snapshot = await _contentLoader.LoadAsync(_testId, _testCatalogService);
        if (snapshot is null)
        {
            return;
        }

        DescriptionText = snapshot.Description;
        CommentText = snapshot.Comment;
        AlgorithmRows.Clear();
        InitAlgorithmRows(snapshot.Algorithm.ToList());
    }
}
