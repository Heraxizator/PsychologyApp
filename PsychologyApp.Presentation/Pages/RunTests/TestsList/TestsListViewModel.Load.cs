using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Pages.RunTests.TestsList;

public partial class TestsListViewModel
{
    private async Task HandleSelectionAsync(TestItem testItem) =>
        await _navigationService.GoToFindProblemAsync(
            testItem.Description,
            testItem.Algorithm,
            testItem.Comment,
            testItem.StartAsync,
            testItem.TestId);

    public async Task InitAsync()
    {
        try
        {
            await _databaseReadySignal.WaitAsync();
            SetInit();

            TestsListLoadResult result = await _testsListLoader.LoadItemsAsync(
                _navigationService,
                HandleSelectionAsync);

            TestItemCollection = new ObservableCollection<TestItem>(result.Items);
            OnPropertyChanged(nameof(TestItemCollection));
            SetDone();

            if (result.ProgressDeferred)
            {
                EnrichProgressInBackgroundAsync(result.Items).FireAndForget();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestsListViewModel init failed.");
            SetFail();
        }
    }

    private async Task EnrichProgressInBackgroundAsync(IReadOnlyList<TestItem> items)
    {
        try
        {
            await _testsListLoader.EnrichProgressAsync(items);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Deferred test progress enrichment failed.");
        }
    }
}
