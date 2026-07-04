using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    public string MoodTrendTitle => AppStrings.ProfileMoodTrendTitle;
    public string MoodTrendHint => AppStrings.ProfileMoodTrendHint;
    public bool ShowMoodTrendHint => !HasMoodTrendChart;

    private IReadOnlyList<MoodChartPoint> _moodChartPoints = [];
    public IReadOnlyList<MoodChartPoint> MoodChartPoints
    {
        get => _moodChartPoints;
        private set => SetProperty(ref _moodChartPoints, value);
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
}
