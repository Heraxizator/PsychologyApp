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
        ContinueSoftCommand = new AsyncCommand(ContinueSoftAsync);
        LoadAsync().FireAndForget();
    }

    public string PageTitle => AppStrings.CrisisHubTitle;
    public string LeadText => AppStrings.CrisisHubLead;
    public string SafetyPlanTitle => AppStrings.CrisisHubSafetyPlanTitle;
    public string SafetyPlanBody => AppStrings.CrisisHubSafetyPlanBody;
    public string HotlineTitle => AppStrings.CrisisHubHotlineTitle;
    public string HotlineRu => AppStrings.CrisisHubHotlineRu;
    public string HotlineIntl => AppStrings.CrisisHubHotlineIntl;
    public string OpenHelplineText => AppStrings.CrisisHubOpenHelpline;
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
    public ICommand ContinueSoftCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(LeadText),
            nameof(SafetyPlanTitle),
            nameof(SafetyPlanBody),
            nameof(HotlineTitle),
            nameof(HotlineRu),
            nameof(HotlineIntl),
            nameof(OpenHelplineText),
            nameof(ContinueSoftText),
            nameof(SpecialistHint));
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

    private Task ContinueSoftAsync() => _navigationService.GoToPracticeTabAsync();
}
