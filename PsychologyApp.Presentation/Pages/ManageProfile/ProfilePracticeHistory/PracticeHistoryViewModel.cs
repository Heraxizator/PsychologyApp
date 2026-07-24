using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfilePracticeHistory;

public sealed class PracticeHistoryViewModel : BaseViewModel
{
    private readonly ProfilePracticeHistoryLoader _loader;
    private readonly INavigationService _navigationService;

    public PracticeHistoryViewModel(
        ProfilePracticeHistoryLoader loader,
        INavigationService navigationService)
    {
        _loader = loader;
        _navigationService = navigationService;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.PracticeHistoryPageTitle;
    public string EmptyText => AppStrings.PracticeHistoryEmpty;
    public ICommand BackCommand { get; }

    private ObservableCollection<PracticeHistoryItem> _items = [];
    public ObservableCollection<PracticeHistoryItem> Items
    {
        get => _items;
        private set
        {
            if (SetProperty(ref _items, value))
            {
                OnPropertyChanged(nameof(HasItems));
                OnPropertyChanged(nameof(ShowEmpty));
            }
        }
    }

    public bool HasItems => Items.Count > 0;
    public bool ShowEmpty => !HasItems;

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(EmptyText), nameof(HasItems), nameof(ShowEmpty));
    }

    private async Task LoadAsync()
    {
        try
        {
            IReadOnlyList<PracticeHistoryItem> loaded = await _loader.LoadAsync(40);
            Items = new ObservableCollection<PracticeHistoryItem>(
                loaded.Select(item => ProfilePracticeHistoryTapFactory.WithTapCommand(item, _navigationService)));
        }
        catch
        {
            Items = [];
        }
    }
}
