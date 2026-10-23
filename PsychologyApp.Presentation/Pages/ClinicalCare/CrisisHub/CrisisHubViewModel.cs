using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Practice;
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
        CallHotlineCommand = new AsyncCommand(() => Lines.LineNumber is { } number ? PhoneCall.DialAsync(number) : Task.CompletedTask);
        CallEmergencyCommand = new AsyncCommand(() => PhoneCall.DialAsync(Lines.Emergency));
        RecheckCommand = new AsyncCommand(() => _navigationService.GoToRiskCheckAsync(AppStrings.RiskCheckSourceManual));
        ContinueSoftCommand = new AsyncCommand(ContinueSoftAsync);
        OpenSafetyPlanCommand = new AsyncCommand(() => _navigationService.GoToSafetyPlanAsync());
        OpenFlashbackCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(TechniqueId.Flashback));
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.CrisisHubTitle;
    public string LeadText => AppStrings.CrisisHubLead;
    public string SafetyPlanTitle => AppStrings.CrisisHubSafetyPlanTitle;
    public string SafetyPlanStep1 => AppStrings.CrisisHubSafetyPlanStep1;
    public string SafetyPlanStep2 => AppStrings.CrisisHubSafetyPlanStep2;
    public string SafetyPlanStep3 => AppStrings.CrisisHubSafetyPlanStep3(Lines.Emergency);
    public string SafetyPlanStepNumber1 => AppStrings.CrisisHubSafetyPlanStepNumber1;
    public string SafetyPlanStepNumber2 => AppStrings.CrisisHubSafetyPlanStepNumber2;
    public string SafetyPlanStepNumber3 => AppStrings.CrisisHubSafetyPlanStepNumber3;
    public string HotlineTitle => AppStrings.CrisisHubHotlineTitle;
    public string HotlineIntl => AppStrings.CrisisHubHotlineIntl;
    // The country is read once, when the screen opens; the line's name follows the interface language.
    private readonly string? _country = DeviceCountry.Code;
    private CrisisLines.Lines Lines => CrisisLines.For(_country);

    public bool HasHotline => Lines.HasLine;
    public string CallHotlineText => Lines.HasLine ? AppStrings.CrisisHubCallHotline(Lines.LineName!, Lines.LineDisplay!) : string.Empty;
    public string CallEmergencyText => AppStrings.CrisisHubCallEmergency(Lines.Emergency);
    public string EmergencyBadge => Lines.Emergency;
    public string OpenHelplineText => AppStrings.CrisisHubOpenHelpline;
    public string RecheckText => AppStrings.CrisisHubRecheck;
    public string ContinueSoftText => AppStrings.CrisisHubContinueSoft;
    public string SpecialistHint => AppStrings.CrisisHubSpecialistHint;
    public string SafetyPlanLinkTitle => AppStrings.CrisisHubSafetyPlanLinkTitle;
    public string SafetyPlanLinkSubtitle => AppStrings.CrisisHubSafetyPlanLinkSubtitle;
    public string FlashbackTitle => AppStrings.CrisisHubFlashbackTitle;
    public string FlashbackSubtitle => AppStrings.CrisisHubFlashbackSubtitle;

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
    public ICommand CallHotlineCommand { get; }
    public ICommand CallEmergencyCommand { get; }
    public ICommand RecheckCommand { get; }
    public ICommand ContinueSoftCommand { get; }
    public ICommand OpenSafetyPlanCommand { get; }

    /// <summary>A flashback can feel like danger now; the way back to the present sits next to the hotlines.</summary>
    public ICommand OpenFlashbackCommand { get; }

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
            nameof(HotlineIntl),
            nameof(CallHotlineText),
            nameof(CallEmergencyText),
            nameof(EmergencyBadge),
            nameof(OpenHelplineText),
            nameof(RecheckText),
            nameof(ContinueSoftText),
            nameof(SpecialistHint),
            nameof(SafetyPlanLinkTitle),
            nameof(SafetyPlanLinkSubtitle),
            nameof(FlashbackTitle),
            nameof(FlashbackSubtitle),
            nameof(ShowContinueSoft));
    }

    private async Task LoadAsync()
    {
        try
        {
            // Same rule as the startup gate: only a current Red counts, so the screen never contradicts why it was opened.
            IsRed = await _clinicalCareService.ShouldRouteToCrisisHubAsync();
        }
        catch
        {
            // This is a safety screen: if the state cannot be read, do not offer the "back to practices" shortcut.
            IsRed = true;
        }
    }

    private static async Task OpenHelplineAsync()
    {
        try
        {
            await Browser.Default.OpenAsync("https://findahelpline.com", BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception bestEffortError)
        {
            // Best-effort external link.
            BestEffort.Report(bestEffortError);
        }
    }


    private async Task ContinueSoftAsync()
    {
        await _navigationService.GoBackAsync();
        await _navigationService.GoToPracticeTabAsync();
    }
}
