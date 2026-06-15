using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public partial class TestsListViewModel
{
    public string PageTitle => AppStrings.TestsDetectorTitle;
    public string EmptyTitle => AppStrings.TestsEmptyTitle;
    public string EmptyBody => AppStrings.TestsEmptyBody;
    public string LoadingText => AppStrings.TestsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string ProfileToolbarText => AppStrings.ProfileTitle;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(EmptyTitle),
            nameof(EmptyBody),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(ProfileToolbarText));
        InitAsync().FireAndForget();
    }
}
