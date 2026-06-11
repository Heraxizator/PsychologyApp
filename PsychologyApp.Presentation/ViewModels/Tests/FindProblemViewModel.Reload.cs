using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;

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

        IReadOnlyList<TestItem> items = await TestCatalogLoader.LoadAllAsync(_navigationService);
        TestItem? item = items.FirstOrDefault(test => string.Equals(test.TestId, _testId, StringComparison.Ordinal));
        if (item is null)
        {
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            DescriptionText = item.Description;
            CommentText = item.Comment;
            AlgorithmRows.Clear();
            InitAlgorithmRows(item.Algorithm);
        });
    }
}
