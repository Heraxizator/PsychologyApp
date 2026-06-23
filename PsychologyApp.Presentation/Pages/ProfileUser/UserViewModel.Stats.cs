namespace PsychologyApp.Presentation.Pages.ProfileUser;

public partial class UserViewModel
{
    private string _techniques_completed_count = "0";
    public string TechniquesCompletedCount
    {
        get => _techniques_completed_count;
        set => SetProperty(ref _techniques_completed_count, value);
    }

    private string _tests_completed_count = "0";
    public string TestsCompletedCount
    {
        get => _tests_completed_count;
        set => SetProperty(ref _tests_completed_count, value);
    }

    private string _streak_count = "0";
    public string StreakCount
    {
        get => _streak_count;
        set => SetProperty(ref _streak_count, value);
    }
}
