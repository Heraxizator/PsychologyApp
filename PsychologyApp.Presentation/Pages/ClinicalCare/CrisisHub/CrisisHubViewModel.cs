using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;

public sealed class CrisisHubViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IClinicalCareService _clinicalCareService;

    public CrisisHubViewModel(
        INavigationService navigationService,
        IClinicalCareService clinicalCareService)
    {
        BindNavigation(navigationService);
        _navigationService = navigationService;
        _clinicalCareService = clinicalCareService;
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
        OpenHelplineCommand = new AsyncCommand(OpenHelplineAsync);
        CallHotlineRuCommand = new AsyncCommand(() => DialAsync(AppStrings.CrisisHubHotlineRuNumber));
        CallEmergencyCommand = new AsyncCommand(() => DialAsync(AppStrings.CrisisHubEmergencyNumber));
        RecheckCommand = new AsyncCommand(() => _navigationService.GoToRiskCheckAsync(AppStrings.RiskCheckSourceManual));
        ContinueSoftCommand = new AsyncCommand(ContinueSoftAsync);
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.CrisisHubTitle;
    public string LeadText => AppStrings.CrisisHubLead;
    public string SafetyPlanTitle => AppStrings.CrisisHubSafetyPlanTitle;
    public string SafetyPlanStep1 => AppStrings.CrisisHubSafetyPlanStep1;
    public string SafetyPlanStep2 => AppStrings.CrisisHubSafetyPlanStep2;
    public string SafetyPlanStep3 => AppStrings.CrisisHubSafetyPlanStep3;
    public string SafetyPlanStepNumber1 => AppStrings.CrisisHubSafetyPlanStepNumber1;
    public string SafetyPlanStepNumber2 => AppStrings.CrisisHubSafetyPlanStepNumber2;
    public string SafetyPlanStepNumber3 => AppStrings.CrisisHubSafetyPlanStepNumber3;
    public string HotlineTitle => AppStrings.CrisisHubHotlineTitle;
    public string HotlineRu => AppStrings.CrisisHubHotlineRu;
    public string HotlineIntl => AppStrings.CrisisHubHotlineIntl;
    public string CallHotlineRuText => AppStrings.CrisisHubCallHotlineRu;
    public string CallEmergencyText => AppStrings.CrisisHubCallEmergency;
    public string EmergencyBadge => AppStrings.CrisisHubEmergencyBadge;
    public string OpenHelplineText => AppStrings.CrisisHubOpenHelpline;
    public string RecheckText => AppStrings.CrisisHubRecheck;
    public string ContinueSoftText => AppStrings.CrisisHubContinueSoft;
    public string SpecialistHint => AppStrings.CrisisHubSpecialistHint;

    private bool _isRed;
    public bool IsRed
    {
        get => _isRed;
        private set
        {
            if (SetProperty(ref _isRed, value))
            {
                OnPropertyChanged(nameof(ShowContinueSoft));
            }
        }
    }

    public bool ShowContinueSoft => !IsRed;

    public ICommand BackCommand { get; }
    public ICommand OpenHelplineCommand { get; }
    public ICommand CallHotlineRuCommand { get; }
    public ICommand CallEmergencyCommand { get; }
    public ICommand RecheckCommand { get; }
    public ICommand ContinueSoftCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(LeadText),
            nameof(SafetyPlanTitle),
            nameof(SafetyPlanStep1),
            nameof(SafetyPlanStep2),
            nameof(SafetyPlanStep3),
            nameof(SafetyPlanStepNumber1),
            nameof(SafetyPlanStepNumber2),
            nameof(SafetyPlanStepNumber3),
            nameof(HotlineTitle),
            nameof(HotlineRu),
            nameof(HotlineIntl),
            nameof(CallHotlineRuText),
            nameof(CallEmergencyText),
            nameof(EmergencyBadge),
            nameof(OpenHelplineText),
            nameof(RecheckText),
            nameof(ContinueSoftText),
            nameof(SpecialistHint),
            nameof(ShowContinueSoft));
    }

    private async Task LoadAsync()
    {
        try
        {
            RiskAssessmentDTO? latest = await _clinicalCareService.GetLatestRiskAssessmentAsync();
            IsRed = latest?.RiskLevel is RiskLevel.Red;
        }
        catch
        {
            IsRed = false;
        }
    }

    private static async Task OpenHelplineAsync()
    {
        try
        {
            await Browser.Default.OpenAsync("https://findahelpline.com", BrowserLaunchMode.SystemPreferred);
        }
        catch
        {
            // Best-effort external link.
        }
    }

    private static async Task DialAsync(string number)
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open(number);
                return;
            }

            await Launcher.Default.OpenAsync($"tel:{number}");
        }
        catch
        {
            // Best-effort dial.
        }
    }

    private async Task ContinueSoftAsync()
    {
        await _navigationService.GoBackAsync();
        await _navigationService.GoToPracticeTabAsync();
    }
}
