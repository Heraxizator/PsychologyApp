using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestHistoryViewModel
{
    private async Task LoadAsync()
    {
        try
        {
            SetInit();
            await _databaseReadySignal.WaitAsync();
            TestHistoryLoadResult result = await _historyLoader.LoadEntriesAsync(
                _testId,
                _testTitle,
                _userProgressService,
                _testCatalogService);

            await UiThread.RunAsync(() =>
            {
                _testTitle = result.Title;
                OnPropertyChanged(nameof(PageTitle));
                HistoryEntries.Clear();
                foreach (TestHistoryEntryItem item in result.Entries)
                {
                    HistoryEntries.Add(item);
                }

                OnPropertyChanged(nameof(HasEntries));
                SetDone();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestHistoryViewModel load failed for {TestId}.", _testId);
            await UiThread.RunAsync(SetFail);
        }
    }
}
