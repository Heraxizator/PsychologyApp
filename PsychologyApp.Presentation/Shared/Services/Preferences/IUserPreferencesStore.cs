using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Shared.Services.Preferences;

public interface IUserPreferencesStore
{
    event Action? Changed;

    UserPreferencesState Load();
    void Save(UserPreferencesState state);
    void ApplyAll();
    void ApplyPreview(UserPreferencesState state);
    void CompleteOnboarding(
        string concern,
        bool? practiceRemindersEnabled = null,
        int? practiceReminderHour = null);
    void ResetOnboardingCompletion();
    void SetPendingTechnique(TechniqueId techniqueId);
    TechniqueId? ConsumePendingTechnique();
    bool HasUsedPhysicsSearch { get; }
    void MarkPhysicsSearchUsed();
    void SetPendingOpenJournal();
    bool ConsumePendingOpenJournal();
    string PersistedLanguage { get; }
    void SetPendingQuoteFeed(string feedKey);
    string? ConsumePendingQuoteFeed();
}
