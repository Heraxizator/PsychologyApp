using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Services.Preferences;

public interface IUserPreferencesStore
{
    event Action? Changed;

    UserPreferencesState Load();
    void Save(UserPreferencesState state);
    void ApplyAll();
    void ApplyPreview(UserPreferencesState state);
    void CompleteOnboarding(string concern);
    void ResetOnboardingCompletion();
    void SetPendingTechnique(TechniqueId techniqueId);
    TechniqueId? ConsumePendingTechnique();
}

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

    public void CompleteOnboarding(string concern) => UserPreferences.CompleteOnboarding(concern);

    public void ResetOnboardingCompletion() => UserPreferences.ResetOnboardingCompletion();

    public void SetPendingTechnique(TechniqueId techniqueId) => UserPreferences.SetPendingTechnique(techniqueId);

    public TechniqueId? ConsumePendingTechnique() => UserPreferences.ConsumePendingTechnique();
}
