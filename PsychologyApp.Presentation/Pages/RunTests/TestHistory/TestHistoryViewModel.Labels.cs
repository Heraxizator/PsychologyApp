using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.RunTests.TestHistory;

public partial class TestHistoryViewModel
{
    public string PageTitle => $"{AppStrings.TestHistoryTitle}: {_testTitle}";
    public string EmptyText => AppStrings.TestHistoryEmpty;
    public string LoadingText => AppStrings.TestsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string RetakeButtonText => AppStrings.TestRetakeButton;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(EmptyText),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(RetakeButtonText));
        LoadAsync().FireAndForget();
    }
}
