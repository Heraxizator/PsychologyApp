using System.Windows.Input;
using PsychologyApp.Application.Body;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Body;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.BodyCheck;

/// <summary>
/// The Body page: where and what is felt, which feelings may be behind it (guesses to check, see <see cref="BodyRules"/>), a note, then
/// practices; and the person's own patterns from earlier check-ins. See docs/body-check.md.
/// </summary>
public sealed class BodyCheckViewModel : BaseViewModel
{
    /// <summary>The same back arrow as on every other pushed page (the page hides the system bar).</summary>
    public ICommand BackCommand { get; }

    private const int RecentShown = 5;

    private readonly INavigationService _navigation;
    private readonly IBodyService _service;
    private readonly IDialogService _dialogs;
    private readonly IDatabaseReadySignal _databaseReady;
    private readonly HashSet<BodyFeeling> _feelings = [];

    private BodyArea? _area;
    private BodySensation? _sensation;
    private int? _intensity;
    private string _note = string.Empty;
    private bool _isSaved;
    private bool _isCrisis;
    private IReadOnlyList<BodyChoiceItem> _guessChips = [];
    private IReadOnlyList<BodyChoiceItem> _otherChips = [];
    private IReadOnlyList<BodyPracticeItem> _practices = [];
    private IReadOnlyList<BodyPatternItem> _patterns = [];
    private IReadOnlyList<BodyEntryItem> _recent = [];

    public BodyCheckViewModel(INavigationService navigation, IBodyService service, IDialogService dialogs, IDatabaseReadySignal databaseReady)
    {
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(GoBackAsync);
        _navigation = navigation;
        _service = service;
        _dialogs = dialogs;
        _databaseReady = databaseReady;

        SelectAreaCommand = new Command<BodyArea>(SelectArea);
        AreaChips = [.. Enum.GetValues<BodyArea>().Select(area => new BodyChoiceItem
        {
            Text = BodyLabels.Area(area),
            Value = area,
            TapCommand = new Command(() => SelectArea(area))
        })];
        SensationChips = [.. Enum.GetValues<BodySensation>().Select(sensation => new BodyChoiceItem
        {
            Text = BodyLabels.Sensation(sensation),
            Value = sensation,
            TapCommand = new Command(() => SelectSensation(sensation))
        })];
        SaveCommand = new AsyncCommand(SaveAsync);
        AgainCommand = new Command(Reset);
        CrisisCommand = new AsyncCommand(() => _navigation.GoToCrisisHubAsync());
    }

    public string PageTitle => AppStrings.BodyTitle;
    public string LeadText => AppStrings.BodyLead;
    public string NotDiagnosisText => AppStrings.BodyNotDiagnosis;
    public string AreaQuestion => AppStrings.BodyAreaQuestion;
    public string SensationQuestion => AppStrings.BodySensationQuestion;
    public string FeelingQuestion => AppStrings.BodyFeelingQuestion;
    public string FeelingHint => AppStrings.BodyFeelingHint;
    public string OtherFeelingsText => AppStrings.BodyOtherFeelings;
    public string PromptVoice => AppStrings.BodyPromptVoice;
    public string PromptBefore => AppStrings.BodyPromptBefore;
    public string NotePlaceholder => AppStrings.BodyNotePlaceholder;
    public string DoctorText => AppStrings.BodyDoctorFirst;
    public string SaveText => AppStrings.BodySave;
    public string SavedText => AppStrings.BodySaved;
    public string CrisisText => AppStrings.BodyCrisis;
    public string CrisisButtonText => AppStrings.CrisisHubTitle;
    public string AgainText => AppStrings.BodyAgain;
    public string PatternsTitle => AppStrings.BodyPatternsTitle;
    public string PatternsEmptyText => AppStrings.BodyPatternsEmpty;
    public string RecentTitle => AppStrings.BodyRecentTitle;


    public IReadOnlyList<BodyChoiceItem> AreaChips { get; }
    public IReadOnlyList<BodyChoiceItem> SensationChips { get; }

    public ICommand SelectAreaCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand AgainCommand { get; }
    public ICommand CrisisCommand { get; }

    public BodyArea? SelectedArea
    {
        get => _area;
        private set
        {
            if (SetProperty(ref _area, value))
            {
                Notify(nameof(HasArea));
            }
        }
    }

    public bool HasArea => _area is not null;
    public bool HasSensation => _sensation is not null;
    public bool IsEditing => !_isSaved;

    public string IntensityTitle => AppStrings.BodyIntensity;
    public string IntensityMinCaption => AppStrings.RatingScaleMin;
    public string IntensityMaxCaption => AppStrings.RatingScaleMax;

    /// <summary>The 0..10 scale as the rating control reads and writes it; empty until the person rates, and then saved as not rated.</summary>
    public string Intensity
    {
        get => _intensity?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
        set
        {
            int? rating = double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsed)
                ? Math.Clamp((int)Math.Round(parsed), 0, 10)
                : null;
            if (SetProperty(ref _intensity, rating))
            {
                Notify(nameof(ShowDoctor));
            }
        }
    }

    /// <summary>Shown above the feelings, so a body signal that may be an illness is never read only as an emotion.</summary>
    public bool ShowDoctor => _sensation is { } sensation && BodyRules.AsksForDoctor(sensation, _intensity);

    public string Note
    {
        get => _note;
        set => SetProperty(ref _note, value ?? string.Empty);
    }

    public IReadOnlyList<BodyChoiceItem> GuessChips
    {
        get => _guessChips;
        private set => SetProperty(ref _guessChips, value);
    }

    public IReadOnlyList<BodyChoiceItem> OtherChips
    {
        get => _otherChips;
        private set => SetProperty(ref _otherChips, value);
    }

    public bool IsSaved
    {
        get => _isSaved;
        private set
        {
            if (SetProperty(ref _isSaved, value))
            {
                Notify(nameof(IsEditing), nameof(ShowPractices));
            }
        }
    }

    public bool IsCrisis
    {
        get => _isCrisis;
        private set
        {
            if (SetProperty(ref _isCrisis, value))
            {
                Notify(nameof(ShowPractices));
            }
        }
    }

    /// <summary>After crisis words the page offers help, not practices.</summary>
    public bool ShowPractices => _isSaved && !_isCrisis;

    public IReadOnlyList<BodyPracticeItem> Practices
    {
        get => _practices;
        private set => SetProperty(ref _practices, value);
    }

    public IReadOnlyList<BodyPatternItem> Patterns
    {
        get => _patterns;
        private set
        {
            if (SetProperty(ref _patterns, value))
            {
                Notify(nameof(HasPatterns));
            }
        }
    }

    public bool HasPatterns => _patterns.Count > 0;

    public IReadOnlyList<BodyEntryItem> Recent
    {
        get => _recent;
        private set
        {
            if (SetProperty(ref _recent, value))
            {
                Notify(nameof(HasRecent), nameof(ShowPatternsEmpty));
            }
        }
    }

    public bool HasRecent => _recent.Count > 0;

    /// <summary>The empty-patterns hint makes sense only once something was noted.</summary>
    public bool ShowPatternsEmpty => HasRecent && !HasPatterns;

    public async Task LoadAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            BodyLog log = await _service.GetLogAsync();
            await UiThread.RunAsync(() => ShowLog(log));
        }
        catch (Exception loadError)
        {
            // The check-in itself works without the history; only the patterns and the list are missing.
            BestEffort.Report(loadError);
        }
    }

    private void SelectArea(BodyArea area)
    {
        UiHaptics.Tick();
        SelectedArea = area;
        foreach (BodyChoiceItem chip in AreaChips)
        {
            chip.IsSelected = Equals(chip.Value, area);
        }

        RebuildFeelings();
    }

    private void SelectSensation(BodySensation sensation)
    {
        UiHaptics.Tick();
        _sensation = sensation;
        foreach (BodyChoiceItem chip in SensationChips)
        {
            chip.IsSelected = Equals(chip.Value, sensation);
        }

        Notify(nameof(HasSensation), nameof(ShowDoctor));
        RebuildFeelings();
    }

    /// <summary>The guesses follow the place and the sensation; feelings already chosen stay chosen.</summary>
    private void RebuildFeelings()
    {
        if (_area is not { } area || _sensation is not { } sensation)
        {
            return;
        }

        IReadOnlyList<BodyFeeling> guesses = BodyRules.Guesses(area, sensation);
        GuessChips = [.. guesses.Select(FeelingChip)];
        OtherChips = [.. Enum.GetValues<BodyFeeling>().Except(guesses).Select(FeelingChip)];
    }

    private BodyChoiceItem FeelingChip(BodyFeeling feeling) => new()
    {
        Text = BodyLabels.Feeling(feeling),
        Value = feeling,
        TapCommand = new Command(() => ToggleFeeling(feeling)),
        IsSelected = _feelings.Contains(feeling)
    };

    /// <summary>Several feelings can be chosen; "I don't know" and a named feeling exclude each other.</summary>
    private void ToggleFeeling(BodyFeeling feeling)
    {
        UiHaptics.Tick();
        if (!_feelings.Remove(feeling))
        {
            if (feeling == BodyFeeling.Unknown)
            {
                _feelings.Clear();
            }
            else
            {
                _feelings.Remove(BodyFeeling.Unknown);
            }

            _feelings.Add(feeling);
        }

        foreach (BodyChoiceItem chip in GuessChips.Concat(OtherChips))
        {
            chip.IsSelected = _feelings.Contains((BodyFeeling)chip.Value);
        }
    }

    private async Task SaveAsync()
    {
        if (_area is not { } area || _sensation is not { } sensation)
        {
            return;
        }

        try
        {
            // In the order shown, so the saved entry reads like the screen did.
            List<BodyFeeling> feelings = [.. GuessChips.Concat(OtherChips).Select(chip => (BodyFeeling)chip.Value).Where(_feelings.Contains)];
            BodyCheckResult result = await _service.SaveAsync(area, sensation, _intensity, feelings, _note);
            BodyLog log = await _service.GetLogAsync();
            await UiThread.RunAsync(() =>
            {
                IsCrisis = result.Crisis;
                Practices = [.. BodyRules.Practices(feelings).Select(Practice)];
                IsSaved = true;
                ShowLog(log);
            });
        }
        catch (Exception saveError)
        {
            BestEffort.Report(saveError);
            await _dialogs.ShowAsync(null, AppStrings.BodySaveError);
        }
    }

    private BodyPracticeItem Practice(TechniqueId id) =>
        new(CompanionContent.TechniqueTitle(id, AppStrings.IsEnglish(AppStrings.Language)), new AsyncCommand(() => _navigation.GoToTechniqueAsync(id)));

    private void Reset()
    {
        _feelings.Clear();
        _sensation = null;
        SelectedArea = null;
        foreach (BodyChoiceItem chip in AreaChips.Concat(SensationChips))
        {
            chip.IsSelected = false;
        }

        GuessChips = [];
        OtherChips = [];
        Intensity = string.Empty;
        Note = string.Empty;
        IsCrisis = false;
        IsSaved = false;
        Notify(nameof(HasSensation), nameof(ShowDoctor));
    }

    private void ShowLog(BodyLog log)
    {
        IReadOnlyList<BodyEntry> readable = log.Readable();
        Patterns = [.. BodyPatterns.Find(readable, TimeZoneInfo.Local).Select(pattern => new BodyPatternItem(
            BodyLabels.Title(pattern.Area, pattern.Sensation),
            AppStrings.BodyPatternCount(pattern.Count),
            pattern.Feeling is { } feeling ? AppStrings.BodyPatternFeeling(BodyLabels.Feeling(feeling)) : null,
            pattern.MostlyAt is { } part ? AppStrings.BodyPatternTime(BodyLabels.Time(part)) : null))];

        Recent = [.. readable.Reverse().Take(RecentShown).Select(entry => new BodyEntryItem(
            BodyLabels.Title(entry.Area, entry.Sensation),
            entry.Intensity is { } intensity ? $"{entry.CreatedUtc.ToLocalTime():g} · {intensity}/10" : $"{entry.CreatedUtc.ToLocalTime():g}",
            Feelings(entry),
            entry.Note,
            new AsyncCommand(() => DeleteAsync(entry.Id))))];
    }

    /// <summary>The feelings this version can name; one from a newer version is left out rather than shown as a raw value.</summary>
    private static string? Feelings(BodyEntry entry)
    {
        List<string> names = [.. entry.Feelings.Where(Enum.IsDefined).Select(BodyLabels.Feeling)];
        return names.Count == 0 ? null : string.Join(", ", names);
    }

    private async Task DeleteAsync(string entryId)
    {
        if (!await _dialogs.AskAsync(AppStrings.BodyDeleteConfirmTitle, AppStrings.BodyDeleteConfirmBody, AppStrings.BodyDeleteConfirmAccept, AppStrings.BodyCancel))
        {
            return;
        }

        try
        {
            await _service.DeleteAsync(entryId);
            await LoadAsync();
        }
        catch (Exception deleteError)
        {
            BestEffort.Report(deleteError);
        }
    }
}
