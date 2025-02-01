using PsychologyApp.Presentation.Modules.Cleaner;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace MobileHelper.ViewModels.CleanViewModels;

public class MusicPlayerViewModel : BaseViewModel
{
    public ObservableCollection<Audio> AllItems { get; private set; } = [];
    public ObservableCollection<Audio> SelectedItems { get; private set; } = [];
    private string search_text = default!;

    public MusicPlayerViewModel()
    {
        this.ModuleName = "Очиститель";
        this.PageName = "Молитвы";

        SetCreated();

        Start = new Command(() =>
        {
            SetInit();

            Init();
        });

        SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
    }

    private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (AllItems.Any())
        {
            SetDone();
        }
    }

    private void Init()
    {
        try
        {
            ObservableCollection<Audio> collection =
            [
                new Audio
                {
                    Name = "Псалом 50",
                    Description = "Читается 3 раза в течение суток",
                    URL = "https://azbyka.ru/audio/audio1/molitvoslov/psalmy/psalom-50.mp3",
                    ClickCommand = new Command(async () => await PlayAudioAsync("https://azbyka.ru/audio/audio1/molitvoslov/psalmy/psalom-50.mp3"))
                },

                new Audio
                {
                    Name = "Отче Наш",
                    Description = "Основная молитва",
                    URL = "https://azbyka.ru/audio/audio1/molitvoslov/gospodu/otche_nash.mp3",
                    ClickCommand = new Command(async () => await PlayAudioAsync("https://azbyka.ru/audio/audio1/molitvoslov/gospodu/otche_nash.mp3"))
                },

                new Audio
                {
                    Name = "Иисусова Молитва",
                    Description = "Основная молитва",
                    URL = "https://azbyka.ru/audio/audio1/molitvoslov/iisusova_molitva_svyato_elizavetinskij_monastyr.mp3",
                    ClickCommand = new Command(async () => await PlayAudioAsync("https://azbyka.ru/audio/audio1/molitvoslov/iisusova_molitva_svyato_elizavetinskij_monastyr.mp3"))
                },

                new Audio
                {
                    Name = "Царю небесный",
                    Description = "Основная молитва",
                    URL = "https://azbyka.ru/wp-content/uploads/2015/08/tsaryu-nebesnyy.mp3",
                    ClickCommand = new Command(async () => await PlayAudioAsync("https://azbyka.ru/wp-content/uploads/2015/08/tsaryu-nebesnyy.mp3"))
                },

                new Audio
                {
                    Name = "Славословие",
                    Description = "Основная молитва",
                    URL = "https://azbyka.ru/audio/audio1/molitvoslov/velikoe.mp3",
                    ClickCommand = new Command(async () => await PlayAudioAsync("https://azbyka.ru/audio/audio1/molitvoslov/velikoe.mp3"))
                },
            ];

            InitItems(collection);
        }
        
        catch (Exception e)
        {
            SetDone();
        }
    }

    private void InitItems(ObservableCollection<Audio> collection)
    {
        foreach (Audio item in collection)
        {
            SelectedItems.Add(item);

            AllItems.Add(item);
        }
    }

    public Task PlayAudioAsync(string url)
    {
        return Launcher.OpenAsync(url);
    }

    public string SearchText
    {
        get => search_text;
        set
        {
            if (search_text != value)
            {
                search_text = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
    }
}
