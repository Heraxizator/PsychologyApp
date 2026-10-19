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
        SetSelfHarmCommand = new Command<object?>(parameter => HasSelfHarmThoughts = ParseYes(parameter));
        SetDisorientationCommand = new Command<object?>(parameter => HasSevereDisorientation = ParseYes(parameter));
        SetSubstanceCommand = new Command<object?>(parameter => HasSubstanceRisk = ParseYes(parameter));
        SetInsomniaCommand = new Command<object?>(parameter => HasSevereInsomnia = ParseYes(parameter));
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
    public string YesLabel => AppStrings.RiskCheckYes;
    public string NoLabel => AppStrings.RiskCheckNo;
    public string AnswerAllText => AppStrings.RiskCheckAnswerAll;

    private bool? _hasSelfHarmThoughts;
    public bool? HasSelfHarmThoughts
    {
        get => _hasSelfHarmThoughts;
        set
        {
            if (SetProperty(ref _hasSelfHarmThoughts, value))
            {
                OnPropertyChanged(nameof(ShowOpenHelpNow));
                OnPropertyChanged(nameof(IsSelfHarmYes));
                OnPropertyChanged(nameof(IsSelfHarmNo));
                NotifyAnswered();
            }
        }
    }

    public bool IsSelfHarmYes => HasSelfHarmThoughts == true;
    public bool IsSelfHarmNo => HasSelfHarmThoughts == false;

    private bool? _hasSevereDisorientation;
    public bool? HasSevereDisorientation
    {
        get => _hasSevereDisorientation;
        set
        {
            if (SetProperty(ref _hasSevereDisorientation, value))
            {
                OnPropertyChanged(nameof(IsDisorientationYes));
                OnPropertyChanged(nameof(IsDisorientationNo));
                NotifyAnswered();
            }
        }
    }

    public bool IsDisorientationYes => HasSevereDisorientation == true;
    public bool IsDisorientationNo => HasSevereDisorientation == false;

    private bool? _hasSubstanceRisk;
    public bool? HasSubstanceRisk
    {
        get => _hasSubstanceRisk;
        set
        {
            if (SetProperty(ref _hasSubstanceRisk, value))
            {
                OnPropertyChanged(nameof(IsSubstanceYes));
                OnPropertyChanged(nameof(IsSubstanceNo));
                NotifyAnswered();
            }
        }
    }

    public bool IsSubstanceYes => HasSubstanceRisk == true;
    public bool IsSubstanceNo => HasSubstanceRisk == false;

    private bool? _hasSevereInsomnia;
    public bool? HasSevereInsomnia
    {
        get => _hasSevereInsomnia;
        set
        {
            if (SetProperty(ref _hasSevereInsomnia, value))
            {
                OnPropertyChanged(nameof(IsInsomniaYes));
                OnPropertyChanged(nameof(IsInsomniaNo));
                NotifyAnswered();
            }
        }
    }

    public bool IsInsomniaYes => HasSevereInsomnia == true;
    public bool IsInsomniaNo => HasSevereInsomnia == false;

    public bool ShowOpenHelpNow => HasSelfHarmThoughts == true;

    /// <summary>
    /// An unanswered question is not a "no": a tap on "Done" before answering used to save "no risk" and silence the next check
    /// for the whole interval. The way to help (<see cref="ShowOpenHelpNow"/>) never waits for the other answers.
    /// </summary>
    public bool CanSubmit =>
        HasSelfHarmThoughts is not null && HasSevereDisorientation is not null && HasSubstanceRisk is not null && HasSevereInsomnia is not null;

    public bool ShowAnswerAll => !CanSubmit;

    private void NotifyAnswered() => Notify(nameof(CanSubmit), nameof(ShowAnswerAll));

    public ICommand SubmitCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand OpenHelpNowCommand { get; }
    public ICommand SetSelfHarmCommand { get; }
    public ICommand SetDisorientationCommand { get; }
    public ICommand SetSubstanceCommand { get; }
    public ICommand SetInsomniaCommand { get; }

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
            nameof(YesLabel),
            nameof(NoLabel),
            nameof(AnswerAllText),
            nameof(ShowOpenHelpNow));
    }

    private static bool ParseYes(object? parameter) =>
        parameter switch
        {
            bool value => value,
            string text when bool.TryParse(text, out bool parsed) => parsed,
            "yes" => true,
            "no" => false,
            _ => false
        };

    private async Task OpenHelpNowAsync()
    {
        await _navigationService.GoToCrisisHubAsync();
    }

    private async Task SubmitAsync()
    {
        if (HasSelfHarmThoughts is not bool selfHarm
            || HasSevereDisorientation is not bool disorientation
            || HasSubstanceRisk is not bool substance
            || HasSevereInsomnia is not bool insomnia)
        {
            return;
        }

        RiskAssessmentDTO assessment = await _clinicalCareService.AssessRiskAsync(
            new RiskAssessmentInput
            {
                HasSelfHarmThoughts = selfHarm,
                HasSevereDisorientation = disorientation,
                HasSubstanceRisk = substance,
                HasSevereInsomnia = insomnia,
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
