using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    public string MoodTrendTitle => AppStrings.ProfileMoodTrendTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;
    public string MoodNotesTitle => AppStrings.ProfileMoodNotesTitle;

    private IReadOnlyList<MoodChartPoint> _moodChartPoints = [];
    public IReadOnlyList<MoodChartPoint> MoodChartPoints
    {
        get => _moodChartPoints;
        private set => SetProperty(ref _moodChartPoints, value);
    }

    private string _moodChartSubtitle = string.Empty;
    public string MoodChartSubtitle
    {
        get => _moodChartSubtitle;
        private set => SetProperty(ref _moodChartSubtitle, value);
    }

    private bool _hasMoodTrendChart;
    public bool HasMoodTrendChart
    {
        get => _hasMoodTrendChart;
        private set
        {
            if (SetProperty(ref _hasMoodTrendChart, value))
            {
                OnPropertyChanged(nameof(ShowMoodTrendHint));
            }
        }
    }

    private IReadOnlyList<MoodNoteItem> _moodNotes = [];
    public IReadOnlyList<MoodNoteItem> MoodNotes
    {
        get => _moodNotes;
        private set
        {
            if (SetProperty(ref _moodNotes, value))
            {
                OnPropertyChanged(nameof(HasMoodNotes));
            }
        }
    }

    public bool HasMoodNotes => MoodNotes.Count > 0;
}
