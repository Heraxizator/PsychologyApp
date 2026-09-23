using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.SafetyPlan;

public sealed class SafetyPlanTextItem
{
    public required string Text { get; init; }
    public required ICommand DeleteCommand { get; init; }
    public string RemoveHint => AppStrings.SafetyPlanTapToRemoveHint;
}

public sealed class SafetyPlanContactItem
{
    public required string Name { get; init; }
    public required string Phone { get; init; }
    public required ICommand CallCommand { get; init; }
    public required ICommand DeleteCommand { get; init; }
    public string CallText => AppStrings.SafetyPlanCallAction;
    public string DeleteText => AppStrings.Remove;
}

public sealed class SafetyPlanViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IClinicalCareService _clinicalCareService;

    public ObservableCollection<SafetyPlanTextItem> WarningSigns { get; } = [];
    public ObservableCollection<SafetyPlanTextItem> CopingStrategies { get; } = [];
    public ObservableCollection<SafetyPlanTextItem> Reasons { get; } = [];
    public ObservableCollection<SafetyPlanContactItem> Contacts { get; } = [];

    public string NewWarningSignText { get; set; } = string.Empty;
    public string NewCopingStrategyText { get; set; } = string.Empty;
    public string NewReasonText { get; set; } = string.Empty;
    public string NewContactName { get; set; } = string.Empty;
    public string NewContactPhone { get; set; } = string.Empty;

    public ICommand BackCommand { get; }
    public ICommand AddWarningSignCommand { get; }
    public ICommand AddCopingStrategyCommand { get; }
    public ICommand AddReasonCommand { get; }
    public ICommand AddContactCommand { get; }

    public string PageTitle => AppStrings.SafetyPlanPageTitle;
    public string LeadText => AppStrings.SafetyPlanLead;
    public string WarningSignsTitle => AppStrings.SafetyPlanWarningSignsTitle;
    public string WarningSignsPlaceholder => AppStrings.SafetyPlanWarningSignsPlaceholder;
    public string CopingTitle => AppStrings.SafetyPlanCopingTitle;
    public string CopingPlaceholder => AppStrings.SafetyPlanCopingPlaceholder;
    public string ReasonsTitle => AppStrings.SafetyPlanReasonsTitle;
    public string ReasonsPlaceholder => AppStrings.SafetyPlanReasonsPlaceholder;
    public string ContactsTitle => AppStrings.SafetyPlanContactsTitle;
    public string ContactNamePlaceholder => AppStrings.SafetyPlanContactNamePlaceholder;
    public string ContactPhonePlaceholder => AppStrings.SafetyPlanContactPhonePlaceholder;

    public SafetyPlanViewModel(
        INavigationService navigationService,
        IClinicalCareService clinicalCareService)
    {
        BindNavigation(navigationService);
        _navigationService = navigationService;
        _clinicalCareService = clinicalCareService;

        BackCommand = new AsyncCommand(() => _navigationService.GoBackAsync());
        AddWarningSignCommand = new AsyncCommand(() => AddTextAsync(WarningSigns, () => NewWarningSignText, value => NewWarningSignText = value, nameof(NewWarningSignText)));
        AddCopingStrategyCommand = new AsyncCommand(() => AddTextAsync(CopingStrategies, () => NewCopingStrategyText, value => NewCopingStrategyText = value, nameof(NewCopingStrategyText)));
        AddReasonCommand = new AsyncCommand(() => AddTextAsync(Reasons, () => NewReasonText, value => NewReasonText = value, nameof(NewReasonText)));
        AddContactCommand = new AsyncCommand(AddContactAsync);

        LoadAsync().FireAndForget();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(LeadText),
            nameof(WarningSignsTitle),
            nameof(WarningSignsPlaceholder),
            nameof(CopingTitle),
            nameof(CopingPlaceholder),
            nameof(ReasonsTitle),
            nameof(ReasonsPlaceholder),
            nameof(ContactsTitle),
            nameof(ContactNamePlaceholder),
            nameof(ContactPhonePlaceholder),
            nameof(AddButtonText));
    }

    private async Task LoadAsync()
    {
        SafetyPlanDTO plan = await _clinicalCareService.GetSafetyPlanAsync();

        WarningSigns.Clear();
        foreach (string text in plan.WarningSigns)
        {
            WarningSigns.Add(BuildTextItem(WarningSigns, text));
        }

        CopingStrategies.Clear();
        foreach (string text in plan.CopingStrategies)
        {
            CopingStrategies.Add(BuildTextItem(CopingStrategies, text));
        }

        Reasons.Clear();
        foreach (string text in plan.Reasons)
        {
            Reasons.Add(BuildTextItem(Reasons, text));
        }

        Contacts.Clear();
        foreach (SafetyPlanContactDTO contact in plan.Contacts)
        {
            Contacts.Add(BuildContactItem(contact.Name, contact.Phone));
        }
    }

    private SafetyPlanTextItem BuildTextItem(ObservableCollection<SafetyPlanTextItem> collection, string text)
    {
        SafetyPlanTextItem? item = null;
        item = new SafetyPlanTextItem
        {
            Text = text,
            DeleteCommand = new AsyncCommand(() => RemoveTextAsync(collection, item!))
        };
        return item;
    }

    private SafetyPlanContactItem BuildContactItem(string name, string phone)
    {
        SafetyPlanContactItem? item = null;
        item = new SafetyPlanContactItem
        {
            Name = name,
            Phone = phone,
            CallCommand = new AsyncCommand(() => DialAsync(phone)),
            DeleteCommand = new AsyncCommand(() => RemoveContactAsync(item!))
        };
        return item;
    }

    private async Task AddTextAsync(
        ObservableCollection<SafetyPlanTextItem> collection,
        Func<string> getValue,
        Action<string> setValue,
        string propertyName)
    {
        string text = getValue().Trim();
        if (text.Length == 0)
        {
            return;
        }

        collection.Add(BuildTextItem(collection, text));
        setValue(string.Empty);
        OnPropertyChanged(propertyName);
        await PersistAsync();
    }

    private async Task RemoveTextAsync(ObservableCollection<SafetyPlanTextItem> collection, SafetyPlanTextItem item)
    {
        collection.Remove(item);
        await PersistAsync();
    }

    private async Task AddContactAsync()
    {
        string name = NewContactName.Trim();
        string phone = NewContactPhone.Trim();
        if (name.Length == 0 && phone.Length == 0)
        {
            return;
        }

        Contacts.Add(BuildContactItem(name, phone));
        NewContactName = string.Empty;
        NewContactPhone = string.Empty;
        OnPropertyChanged(nameof(NewContactName));
        OnPropertyChanged(nameof(NewContactPhone));
        await PersistAsync();
    }

    private async Task RemoveContactAsync(SafetyPlanContactItem item)
    {
        Contacts.Remove(item);
        await PersistAsync();
    }

    private async Task PersistAsync()
    {
        SafetyPlanDTO plan = new()
        {
            WarningSigns = WarningSigns.Select(item => item.Text).ToArray(),
            CopingStrategies = CopingStrategies.Select(item => item.Text).ToArray(),
            Reasons = Reasons.Select(item => item.Text).ToArray(),
            Contacts = Contacts.Select(item => new SafetyPlanContactDTO { Name = item.Name, Phone = item.Phone }).ToArray()
        };

        await _clinicalCareService.SaveSafetyPlanAsync(plan);
    }

    private static async Task DialAsync(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return;
        }

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
}
