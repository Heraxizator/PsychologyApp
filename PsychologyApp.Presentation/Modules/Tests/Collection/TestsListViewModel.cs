using PsychologyApp.Application.Services.UserProgress;
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
    private readonly IUserProgressService _userProgressService;

    public string PageTitle => AppStrings.TestsDetectorTitle;
    public string EmptyTitle => AppStrings.TestsEmptyTitle;
    public string EmptyBody => AppStrings.TestsEmptyBody;

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

    public TestsListViewModel(
        INavigation navigation,
        INavigationService navigationService,
        IUserProgressService userProgressService)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        BindNavigation(navigation, navigationService);
        UserPreferences.Changed += OnPreferencesChanged;
        InitAsync().FireAndForget();
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(EmptyTitle));
        OnPropertyChanged(nameof(EmptyBody));
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
            await AppReadiness.DatabaseReadyAsync;

            SetInit();

            IReadOnlyList<TestItem> items = await TestCatalogLoader.LoadAllAsync(_navigationService);
            foreach (TestItem item in items)
            {
                if (string.IsNullOrWhiteSpace(item.TestId))
                {
                    continue;
                }

                var latest = await _userProgressService.GetLatestTestResultAsync(item.TestId);
                if (latest is not null)
                {
                    item.LastResultSummary = AppStrings.TestLastResult(latest.Summary);
                }
            }

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
