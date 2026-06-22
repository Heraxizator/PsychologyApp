using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Motivator;

public partial class QuoteViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;

    public bool HasInitialized => _initialized;

    public Task EnsureInitializedAsync() =>
        _initialized ? Task.CompletedTask : RunInitAsync(seedNewQuote: true);

    public Task ReloadFromPullAsync() => RunInitAsync(seedNewQuote: false);

    private async Task RunInitAsync(bool seedNewQuote)
    {
        await _initGate.WaitAsync();
        try
        {
            if (await InitAsync(seedNewQuote))
            {
                _initialized = true;
                _feedLanguage = UserPreferences.GetPersistedLanguage();
            }
        }
        finally
        {
            _initGate.Release();
        }
    }
}
