namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

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

    private string _active_days_count = "0";
    public string ActiveDaysCount
    {
        get => _active_days_count;
        set => SetProperty(ref _active_days_count, value);
    }
}
