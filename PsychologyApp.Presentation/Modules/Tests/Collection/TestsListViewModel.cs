using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Tests.Collection;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class TestsListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public ObservableCollection<TestItem> TestItemCollection { get; private set; } = [];

    private TestItem? _selectedTestItem;
    public TestItem? SelectedTestItem
    {
        get => _selectedTestItem;
        set
        {
            if (!SetProperty(ref _selectedTestItem, value) || value is null)
            {
                return;
            }

            TestItem selected = value;
            _selectedTestItem = null;
            OnPropertyChanged(nameof(SelectedTestItem));
            HandleSelectionAsync(selected).FireAndForget();
        }
    }

    public TestsListViewModel(INavigation navigation, INavigationService navigationService)
    {
        _navigationService = navigationService;
        BindNavigation(navigation, navigationService);
        InitAsync().FireAndForget();
    }

    private async Task HandleSelectionAsync(TestItem testItem) =>
        await _navigationService.GoToFindProblemAsync(
            testItem.Description,
            testItem.Algorithm,
            testItem.Comment,
            testItem.Action);

    public async Task InitAsync()
    {
        try
        {
            SetInit();

            IReadOnlyList<TestItem> items = await TestCatalogLoader.LoadAllAsync(_navigationService);
            TestItemCollection = new ObservableCollection<TestItem>(items);
            OnPropertyChanged(nameof(TestItemCollection));

            SetDone();
        }
        catch (Exception)
        {
            SetFail();
        }
    }
}

