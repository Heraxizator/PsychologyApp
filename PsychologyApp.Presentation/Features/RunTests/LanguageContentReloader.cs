using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed class LanguageContentReloader
{
    public const int DefaultQuoteFeedCount = 20;

    private readonly IQuotService _quotService;
    private readonly CachedReasonContentProvider _reasonCache;
    private readonly CachedQuotContentProvider _quotCache;
    private readonly CachedTestCatalogService _testCatalogCache;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string _lastPersistedLanguage = UserPreferences.DefaultLanguage;
    private Task _reloadTask = Task.CompletedTask;

    public LanguageContentReloader(
        IQuotService quotService,
        CachedReasonContentProvider reasonCache,
        CachedQuotContentProvider quotCache,
        CachedTestCatalogService testCatalogCache)
    {
        _quotService = quotService;
        _reasonCache = reasonCache;
        _quotCache = quotCache;
        _testCatalogCache = testCatalogCache;
        _lastPersistedLanguage = UserPreferences.GetPersistedLanguage();
        UserPreferences.Changed += OnPreferencesChanged;
    }

    public Task EnsureReloadedAsync() => _reloadTask;

    private void OnPreferencesChanged()
    {
        string persistedLanguage = UserPreferences.GetPersistedLanguage();
        if (string.Equals(_lastPersistedLanguage, persistedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _lastPersistedLanguage = persistedLanguage;
        _reasonCache.Invalidate();
        _quotCache.Invalidate();
        _testCatalogCache.Invalidate();
        _reloadTask = ReseedQuotesAsync();
    }

    private async Task ReseedQuotesAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await _quotService.ReseedFeedAsync(DefaultQuoteFeedCount);
        }
        finally
        {
            _gate.Release();
        }
    }
}
