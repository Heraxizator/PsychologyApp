using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;


namespace PsychologyApp.Presentation.Shared.Services.Preferences;

public sealed class MauiUserPreferencesStore : IUserPreferencesStore
{
    public event Action? Changed
    {
        add => UserPreferences.Changed += value;
        remove => UserPreferences.Changed -= value;
    }

    public UserPreferencesState Load() => UserPreferences.Load();

    public void Save(UserPreferencesState state) => UserPreferences.Save(state);

    public void ApplyAll() => UserPreferences.ApplyAll();

    public void ApplyPreview(UserPreferencesState state) => UserPreferences.ApplyPreview(state);

    public void CompleteOnboarding(
        string concern,
        bool? practiceRemindersEnabled = null,
        int? practiceReminderHour = null) =>
        UserPreferences.CompleteOnboarding(concern, practiceRemindersEnabled, practiceReminderHour);

    public void ResetOnboardingCompletion() => UserPreferences.ResetOnboardingCompletion();

    public void SetPendingTechnique(TechniqueId techniqueId) => UserPreferences.SetPendingTechnique(techniqueId);

    public TechniqueId? ConsumePendingTechnique() => UserPreferences.ConsumePendingTechnique();

    public bool HasUsedPhysicsSearch => UserPreferences.HasUsedPhysicsSearch;

    public void MarkPhysicsSearchUsed() => UserPreferences.MarkPhysicsSearchUsed();

    public void SetPendingOpenJournal() => UserPreferences.SetPendingOpenJournal();

    public bool ConsumePendingOpenJournal() => UserPreferences.ConsumePendingOpenJournal();

    public string PersistedLanguage => UserPreferences.GetPersistedLanguage();

    public void SetPendingQuoteFeed(string feedKey) => UserPreferences.SetPendingQuoteFeed(feedKey);

    public string? ConsumePendingQuoteFeed() => UserPreferences.ConsumePendingQuoteFeed();
}
