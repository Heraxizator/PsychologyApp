using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Models.Clean;

public static class MusicPlaylist
{
    public static ObservableCollection<Audio> CreateDefault() => PrayerCatalog.CreateFlatList();
}
