using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;

public partial class PhysicsSearchViewModel
{
    public string PageTitle => AppStrings.PhysicsSearchTitle;
    public string SearchToolbarText => AppStrings.PhysicsSearchToolbar;
    public string ProblemLabel => AppStrings.PhysicsProblemLabel;
    public string IllnessPlaceholder => AppStrings.PhysicsIllnessPlaceholder;
    public string EmptySearchHint => AppStrings.PhysicsEmptySearchHint;
    public string EmptySearchSubhint => AppStrings.PhysicsEmptySearchSubhint;
    public string NoResultsHint => AppStrings.PhysicsNoResultsHint;
    public string NoResultsSubhint => AppStrings.PhysicsNoResultsSubhint;
    public string LoadingText => AppStrings.PhysicsLoadingText;
    public string SearchFilteringText => AppStrings.PhysicsSearchFilteringText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string SolutionHeader => AppStrings.PhysicsSolutionHeader;
    public string RecommendedPracticesLabel => AppStrings.PhysicsRecommendedPractices;
    public string TryPracticeLabel => AppStrings.PhysicsTryPractice;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchToolbarText),
            nameof(ProblemLabel),
            nameof(IllnessPlaceholder),
            nameof(EmptySearchHint),
            nameof(EmptySearchSubhint),
            nameof(NoResultsHint),
            nameof(NoResultsSubhint),
            nameof(LoadingText),
            nameof(SearchFilteringText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(SolutionHeader),
            nameof(RecommendedPracticesLabel),
            nameof(TryPracticeLabel));

        string currentLanguage = UserPreferences.GetPersistedLanguage();
        if (string.Equals(_reasonsLanguage, currentLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _reasonsLanguage = currentLanguage;
        if (_initialized)
        {
            ReloadAsync().FireAndForget();
        }
    }
}
