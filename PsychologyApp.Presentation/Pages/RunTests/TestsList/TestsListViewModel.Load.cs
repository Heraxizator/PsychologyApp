using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Entities.Test;
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

            IReadOnlyList<TestItem> items = await _testsListLoader.LoadItemsAsync(
                _navigationService,
                HandleSelectionAsync);

            TestItemCollection = new ObservableCollection<TestItem>(items);
            OnPropertyChanged(nameof(TestItemCollection));
            SetDone();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestsListViewModel init failed.");
            SetFail();
        }
    }
}
