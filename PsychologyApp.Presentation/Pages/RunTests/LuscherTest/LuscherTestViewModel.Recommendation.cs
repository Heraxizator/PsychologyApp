using PsychologyApp.Application.Tests;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.LuscherTest;

public partial class LuscherTestViewModel
{
    private TechniqueId? _recommendedTechnique;

    public bool HasRecommendation { get; private set; }
    public string RecommendationHint { get; private set; } = string.Empty;
    public string RecommendationTitle { get; private set; } = string.Empty;
    public string RecommendationSubtitle { get; private set; } = string.Empty;
    public string RecommendationTheme { get; private set; } = string.Empty;
    public string RecommendationIconName { get; private set; } = string.Empty;
    public ICommand TryTechniqueCommand { get; private set; } = default!;

    private void InitializeRecommendation()
    {
        TryTechniqueCommand = new AsyncCommand(TryRecommendedTechniqueAsync, () => _recommendedTechnique is not null);
    }

    private Task TryRecommendedTechniqueAsync()
    {
        if (_recommendedTechnique is not TechniqueId techniqueId || NavigationService is null)
        {
            return Task.CompletedTask;
        }

        return NavigationService.GoToTechniqueAsync(techniqueId);
    }

    private async Task LoadRecommendationAsync(int coValue)
    {
        if (_techniqueCatalog is null)
        {
            return;
        }

        _recommendedTechnique = LuscherScoreRecommendation.RecommendTechnique(coValue);
        RecommendationHint = AppStrings.TestsResultRecommendationHint;

        try
        {
            TechniqueDefinition definition = await _techniqueCatalog.GetAsync(_recommendedTechnique.Value);
            RecommendationTitle = definition.ListTitle;
            RecommendationSubtitle = definition.ListSubtitle;
            RecommendationTheme = definition.Theme;
            RecommendationIconName = definition.ListIcon;
            HasRecommendation = true;
        }
        catch (Exception)
        {
            _recommendedTechnique = null;
            HasRecommendation = false;
            RecommendationHint = string.Empty;
        }

        await UiThread.RunAsync(() =>
            Notify(
                nameof(HasRecommendation),
                nameof(RecommendationHint),
                nameof(RecommendationTitle),
                nameof(RecommendationSubtitle),
                nameof(RecommendationTheme),
                nameof(RecommendationIconName)));
    }
}
