using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.TestsList;

public partial class TestsListViewModel
{
    public string PageTitle => AppStrings.TestsDetectorTitle;
    public string SectionTitle => AppStrings.TestsListSectionTitle;
    public string SectionSubtitle => AppStrings.TestsListSectionSubtitle;
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
            nameof(SectionTitle),
            nameof(SectionSubtitle),
            nameof(EmptyTitle),
            nameof(EmptyBody),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(ProfileToolbarText));
        InitAsync().FireAndForget();
    }
}
