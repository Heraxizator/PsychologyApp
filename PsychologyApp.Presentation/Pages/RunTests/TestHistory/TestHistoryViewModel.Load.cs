using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.Pages.RunTests.TestHistory;

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

                ChartPoints = result.ChartPoints;
                ChartDomainMin = result.ChartDomainMin;
                ChartDomainMax = result.ChartDomainMax;
                ChartSubtitle = result.ChartSubtitle;
                OnPropertyChanged(nameof(ChartPoints));
                OnPropertyChanged(nameof(ChartDomainMin));
                OnPropertyChanged(nameof(ChartDomainMax));
                OnPropertyChanged(nameof(ChartSubtitle));
                OnPropertyChanged(nameof(HasChart));
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
