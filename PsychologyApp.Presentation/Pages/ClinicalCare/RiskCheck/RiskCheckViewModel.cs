using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;

public sealed class RiskCheckViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IClinicalCareService _clinicalCareService;
    private readonly string _source;
    private readonly Func<RiskAssessmentDTO, Task>? _onCompleted;

    public RiskCheckViewModel(
        INavigationService navigationService,
        IClinicalCareService clinicalCareService,
        string source,
        Func<RiskAssessmentDTO, Task>? onCompleted = null)
    {
        BindNavigation(navigationService);
        _navigationService = navigationService;
        _clinicalCareService = clinicalCareService;
        _source = string.IsNullOrWhiteSpace(source) ? AppStrings.RiskCheckSourceManual : source;
        _onCompleted = onCompleted;
        SubmitCommand = new AsyncCommand(SubmitAsync);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        OpenHelpNowCommand = new AsyncCommand(OpenHelpNowAsync);
    }

    public string PageTitle => AppStrings.RiskCheckTitle;
    public string LeadText => AppStrings.RiskCheckLead;
    public string Subtitle => AppStrings.RiskCheckSubtitle;
    public string SelfHarmLabel => AppStrings.RiskCheckSelfHarm;
    public string DisorientationLabel => AppStrings.RiskCheckDisorientation;
    public string SubstanceLabel => AppStrings.RiskCheckSubstance;
    public string InsomniaLabel => AppStrings.RiskCheckInsomnia;
    public string SubmitText => AppStrings.RiskCheckSubmit;
    public string OpenHelpNowText => AppStrings.RiskCheckOpenHelpNow;

    private bool _hasSelfHarmThoughts;
    public bool HasSelfHarmThoughts
    {
        get => _hasSelfHarmThoughts;
        set
        {
            if (SetProperty(ref _hasSelfHarmThoughts, value))
            {
                OnPropertyChanged(nameof(ShowOpenHelpNow));
            }
        }
    }

    private bool _hasSevereDisorientation;
    public bool HasSevereDisorientation
    {
        get => _hasSevereDisorientation;
        set => SetProperty(ref _hasSevereDisorientation, value);
    }

    private bool _hasSubstanceRisk;
    public bool HasSubstanceRisk
    {
        get => _hasSubstanceRisk;
        set => SetProperty(ref _hasSubstanceRisk, value);
    }

    private bool _hasSevereInsomnia;
    public bool HasSevereInsomnia
    {
        get => _hasSevereInsomnia;
        set => SetProperty(ref _hasSevereInsomnia, value);
    }

    public bool ShowOpenHelpNow => HasSelfHarmThoughts;

    public ICommand SubmitCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand OpenHelpNowCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(LeadText),
            nameof(Subtitle),
            nameof(SelfHarmLabel),
            nameof(DisorientationLabel),
            nameof(SubstanceLabel),
            nameof(InsomniaLabel),
            nameof(SubmitText),
            nameof(OpenHelpNowText),
            nameof(ShowOpenHelpNow));
    }

    private async Task OpenHelpNowAsync()
    {
        await _navigationService.GoToCrisisHubAsync();
    }

    private async Task SubmitAsync()
    {
        RiskAssessmentDTO assessment = await _clinicalCareService.AssessRiskAsync(
            new RiskAssessmentInput
            {
                HasSelfHarmThoughts = HasSelfHarmThoughts,
                HasSevereDisorientation = HasSevereDisorientation,
                HasSubstanceRisk = HasSubstanceRisk,
                HasSevereInsomnia = HasSevereInsomnia,
                Source = _source
            });

        if (_onCompleted is not null)
        {
            await _onCompleted(assessment);
            return;
        }

        await _navigationService.GoBackAsync();
        if (assessment.RiskLevel is RiskLevel.Red or RiskLevel.Amber)
        {
            await _navigationService.GoToCrisisHubAsync();
        }
    }
}
