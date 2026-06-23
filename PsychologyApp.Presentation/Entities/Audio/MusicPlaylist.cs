using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Entities.Audio;

public static class MusicPlaylist
{
    public static ObservableCollection<Audio> CreateDefault() => PrayerCatalog.CreateFlatList();
}
