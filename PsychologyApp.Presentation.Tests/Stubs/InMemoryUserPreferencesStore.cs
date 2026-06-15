using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Preferences;

namespace PsychologyApp.Presentation.Tests;

public sealed class InMemoryUserPreferencesStore : IUserPreferencesStore
{
    private UserPreferencesState _state = new();
    private TechniqueId? _pendingTechnique;

    public event Action? Changed;

    public UserPreferencesState Load() => _state;

    public void Save(UserPreferencesState state)
    {
        _state = state;
        Changed?.Invoke();
    }

    public void ApplyAll()
    {
    }

    public void ApplyPreview(UserPreferencesState state) => _state = state;

    public void CompleteOnboarding(string concern)
    {
        _state = new UserPreferencesState
        {
            Language = _state.Language,
            Theme = _state.Theme,
            Color = _state.Color,
            Form = _state.Form,
            Size = _state.Size,
            IsBold = _state.IsBold,
            HasCompletedOnboarding = true,
            OnboardingConcern = concern
        };
        Changed?.Invoke();
    }

    public void ResetOnboardingCompletion()
    {
        _state = new UserPreferencesState
        {
            Language = _state.Language,
            Theme = _state.Theme,
            Color = _state.Color,
            Form = _state.Form,
            Size = _state.Size,
            IsBold = _state.IsBold,
            HasCompletedOnboarding = false,
            OnboardingConcern = _state.OnboardingConcern
        };
        Changed?.Invoke();
    }

    public void SetPendingTechnique(TechniqueId techniqueId) => _pendingTechnique = techniqueId;

    public TechniqueId? ConsumePendingTechnique()
    {
        TechniqueId? pending = _pendingTechnique;
        _pendingTechnique = null;
        return pending;
    }
}
