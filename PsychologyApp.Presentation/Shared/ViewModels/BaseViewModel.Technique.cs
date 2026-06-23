using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.UI.Components;
using MvvmHelpers;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.ViewModels;

public partial class BaseViewModel
{
    protected INavigationService? NavigationService { get; private set; }
    protected IUserPreferencesStore UserPreferencesStore { get; private set; } = new MauiUserPreferencesStore();
    protected string? TheoryInfo { get; set; }
    private TechniqueId? _appliedTechniqueId;

    public string TechniquePageTitle => AppStrings.TechniqueTitle;
    public string TheoryToolbarText => AppStrings.TechniqueTheory;
    public string FormSectionTitle => AppStrings.FormLabel;
    public string AddButtonText => AppStrings.Add;

    public ObservableRangeCollection<string> Algorithm { get; protected set; } = [];

    public List<EntryItem> Entries { get; protected set; } = [];

    protected void BindNavigation(INavigationService navigationService)
    {
        NavigationService = navigationService;
        Finish = new AsyncCommand(GoBackAsync);
        Theory = new Command(() =>
        {
            string content = TheoryInfo ?? string.Empty;
            navigationService.GoToTheoryAsync(content, _appliedTechniqueId).FireAndForget();
        });
    }

    private void HandlePreferencesChanged()
    {
        OnUiPreferencesChanged();
        RefreshLocalizedProperties();
    }

    private void OnUiPreferencesChanged()
    {
        Notify(
            nameof(TechniquePageTitle),
            nameof(TheoryToolbarText),
            nameof(FormSectionTitle),
            nameof(AddButtonText));

        if (_appliedTechniqueId is TechniqueId techniqueId)
        {
            ApplyTechniqueCore(techniqueId);
        }
    }

    protected Task GoBackAsync() => NavigationService!.GoBackAsync();

    protected Task GoToRootAsync() => NavigationService!.GoToRootAsync();

    protected void ApplyTechnique(TechniqueId id)
    {
        _appliedTechniqueId = id;
        ApplyTechniqueCore(id);
    }

    private void ApplyTechniqueCore(TechniqueId id)
    {
        TechniqueDefinition def = TechniqueCatalog.Get(id);
        ModuleName = def.ModuleName;
        PageName = def.PageName;
        Algorithm.Clear();
        Algorithm.AddRange(def.Algorithm);
        TheoryInfo = def.TheoryInfo;
        if (def.Entries is not null)
        {
            Entries.Clear();
            foreach (EntryItem entry in def.Entries)
            {
                Entries.Add(entry.CloneEmpty());
            }

            OnPropertyChanged(nameof(Entries));
        }

        OnTechniqueContentChanged();
    }

    protected virtual void OnTechniqueContentChanged()
    {
    }
}
