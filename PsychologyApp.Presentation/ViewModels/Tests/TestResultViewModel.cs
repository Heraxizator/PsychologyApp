using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Tests;

public class TestResultViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly TechniqueId? _recommendedTechnique;

    public string PageTitle => AppStrings.TestsResultPageTitle;
    public string ScoreTitle => AppStrings.TestsResultTitle(Score);
    public string Interpretation { get; }
    public int Score { get; }
    public bool HasRecommendation => _recommendedTechnique is not null;
    public string RecommendationHint => AppStrings.TestsResultRecommendationHint;
    public string FinishButtonText => AppStrings.TestsFinishButton;
    public string TryTechniqueButtonText => AppStrings.TestTryTechnique;

    public ICommand FinishCommand { get; }
    public ICommand TryTechniqueCommand { get; }

    public TestResultViewModel(
        INavigation navigation,
        INavigationService navigationService,
        TestResultInfo result)
    {
        _navigationService = navigationService;
        _recommendedTechnique = result.RecommendedTechnique;
        Score = result.Score;
        Interpretation = result.Interpretation;

        BindNavigation(navigation, navigationService);

        FinishCommand = new AsyncCommand(FinishAsync);
        TryTechniqueCommand = new AsyncCommand(TryTechniqueAsync);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ScoreTitle),
            nameof(RecommendationHint),
            nameof(FinishButtonText),
            nameof(TryTechniqueButtonText));
    }

    private Task FinishAsync() => _navigationService.GoToRootAsync();

    private async Task TryTechniqueAsync()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }
}
