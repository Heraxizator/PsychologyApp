using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Clean;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Clean;

public class MusicPlayerViewModel : BaseViewModel
{
    private readonly ILogger<MusicPlayerViewModel> _logger;

    public ObservableCollection<Audio> AllItems { get; private set; } = [];
    public ObservableCollection<Audio> SelectedItems { get; private set; } = [];
    private string search_text = string.Empty;

    public string PageTitle => AppStrings.ShellTabCleaner;
    public string PrayerCollectionLabel => AppStrings.CleanerPrayerCollection;
    public string LoadLabel => AppStrings.CleanerLoad;
    public string SearchingPrayersText => AppStrings.CleanerSearchingPrayers;
    public string MoreInfoHeader => AppStrings.TestsMoreInfo;
    public string MoreInfoBody => AppStrings.CleanerMoreInfoBody;
    public string LoadFailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    public MusicPlayerViewModel(ILogger<MusicPlayerViewModel> logger)
    {
        _logger = logger;
        ModuleName = AppStrings.ShellTabCleaner;
        PageName = AppStrings.CleanerPrayersPage;

        SetCreated();

        Start = new AsyncCommand(LoadPlaylistAsync);
        SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(PrayerCollectionLabel),
            nameof(LoadLabel),
            nameof(SearchingPrayersText),
            nameof(MoreInfoHeader),
            nameof(MoreInfoBody),
            nameof(LoadFailedText),
            nameof(RetryText));

        if (IsDone && SelectedItems.Count > 0)
        {
            ObservableCollection<Audio> localized = MusicPlaylist.CreateDefault();
            for (int i = 0; i < SelectedItems.Count && i < localized.Count; i++)
            {
                SelectedItems[i].Name = localized[i].Name;
                SelectedItems[i].Description = localized[i].Description;
            }
        }
    }

    private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (AllItems.Any())
        {
            SetDone();
        }
    }

    private async Task LoadPlaylistAsync()
    {
        try
        {
            SetInit();
            await Task.Yield();

            ObservableCollection<Audio> collection = MusicPlaylist.CreateDefault();
            foreach (Audio item in collection)
            {
                item.ClickCommand = new AsyncCommand(() => PlayAudioAsync(item.URL));
            }

            InitItems(collection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load music playlist.");
            SetFail();
        }
    }

    private void InitItems(ObservableCollection<Audio> collection)
    {
        SelectedItems.Clear();
        AllItems.Clear();

        foreach (Audio item in collection)
        {
            SelectedItems.Add(item);
            AllItems.Add(item);
        }
    }

    public Task PlayAudioAsync(string url) => Launcher.OpenAsync(url);

    public string SearchText
    {
        get => search_text;
        set => SetProperty(ref search_text, value);
    }
}
