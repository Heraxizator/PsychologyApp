using PsychologyApp.Presentation.Shared.Common;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Entities.Audio;

public static class PrayerCatalog
{
    public static ObservableCollection<Audio> CreateFlatList()
    {
        ObservableCollection<Audio> items = [];

        foreach ((string category, Audio audio) in EnumerateDefinitions())
        {
            audio.Category = category;
            items.Add(audio);
        }

        return items;
    }

    private static IEnumerable<(string Category, Audio Audio)> EnumerateDefinitions()
    {
        yield return (AppStrings.CleanerCategoryMorning, new Audio
        {
            Name = AppStrings.CleanerHeavenlyKing,
            Description = AppStrings.CleanerHeavenlyKingDesc,
            URL = "https://azbyka.ru/wp-content/uploads/2015/08/tsaryu-nebesnyy.mp3"
        });

        yield return (AppStrings.CleanerCategoryPenitential, new Audio
        {
            Name = AppStrings.CleanerPsalm50,
            Description = AppStrings.CleanerPsalm50Desc,
            URL = "https://azbyka.ru/audio/audio1/molitvoslov/psalmy/psalom-50.mp3"
        });

        yield return (AppStrings.CleanerCategoryCore, new Audio
        {
            Name = AppStrings.CleanerOurFather,
            Description = AppStrings.CleanerOurFatherDesc,
            URL = "https://azbyka.ru/audio/audio1/molitvoslov/gospodu/otche_nash.mp3"
        });

        yield return (AppStrings.CleanerCategoryCore, new Audio
        {
            Name = AppStrings.CleanerJesusPrayer,
            Description = AppStrings.CleanerJesusPrayerDesc,
            URL = "https://azbyka.ru/audio/audio1/molitvoslov/iisusova_molitva_svyato_elizavetinskij_monastyr.mp3"
        });

        yield return (AppStrings.CleanerCategoryCore, new Audio
        {
            Name = AppStrings.CleanerDoxology,
            Description = AppStrings.CleanerDoxologyDesc,
            URL = "https://azbyka.ru/audio/audio1/molitvoslov/velikoe.mp3"
        });
    }
}
