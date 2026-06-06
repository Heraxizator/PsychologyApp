using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Clean;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.ViewModels.Clean;

public static class MusicPlaylist
{
    public static ObservableCollection<Audio> CreateDefault()
    {
        return
        [
            new Audio
            {
                Name = AppStrings.CleanerPsalm50,
                Description = AppStrings.CleanerPsalm50Desc,
                URL = "https://azbyka.ru/audio/audio1/molitvoslov/psalmy/psalom-50.mp3"
            },
            new Audio
            {
                Name = AppStrings.CleanerOurFather,
                Description = AppStrings.CleanerPrayerMain,
                URL = "https://azbyka.ru/audio/audio1/molitvoslov/gospodu/otche_nash.mp3"
            },
            new Audio
            {
                Name = AppStrings.CleanerJesusPrayer,
                Description = AppStrings.CleanerPrayerMain,
                URL = "https://azbyka.ru/audio/audio1/molitvoslov/iisusova_molitva_svyato_elizavetinskij_monastyr.mp3"
            },
            new Audio
            {
                Name = AppStrings.CleanerHeavenlyKing,
                Description = AppStrings.CleanerPrayerMain,
                URL = "https://azbyka.ru/wp-content/uploads/2015/08/tsaryu-nebesnyy.mp3"
            },
            new Audio
            {
                Name = AppStrings.CleanerDoxology,
                Description = AppStrings.CleanerPrayerMain,
                URL = "https://azbyka.ru/audio/audio1/molitvoslov/velikoe.mp3"
            }
        ];
    }
}
