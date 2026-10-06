using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Reason;
using PsychologyApp.Application.Tests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Common.Localization;

public sealed class LanguageContentReloader
{
    private readonly IUserPreferencesStore _preferences;
    public const int DefaultQuoteFeedCount = 20;

    private readonly IQuotService _quotService;
    private readonly CachedReasonContentProvider _reasonCache;
    private readonly CachedQuotContentProvider _quotCache;
    private readonly CachedTestCatalogProvider _testCatalogCache;
    private readonly CachedTechniqueCatalogProvider _techniqueCatalogCache;
    private readonly ITechniqueCatalogService _techniqueCatalogService;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string _lastPersistedLanguage = UserPreferences.DefaultLanguage;
    private Task _reloadTask = Task.CompletedTask;

    public LanguageContentReloader(
        IQuotService quotService,
        CachedReasonContentProvider reasonCache,
        CachedQuotContentProvider quotCache,
        CachedTestCatalogProvider testCatalogCache,
        CachedTechniqueCatalogProvider techniqueCatalogCache,
        ITechniqueCatalogService techniqueCatalogService,
        IUserPreferencesStore preferences)
    {
        _preferences = preferences;
        _quotService = quotService;
        _reasonCache = reasonCache;
        _quotCache = quotCache;
        _testCatalogCache = testCatalogCache;
        _techniqueCatalogCache = techniqueCatalogCache;
        _techniqueCatalogService = techniqueCatalogService;
        _lastPersistedLanguage = _preferences.PersistedLanguage;
        _preferences.Changed += OnPreferencesChanged;
    }

    public Task EnsureReloadedAsync() => _reloadTask;

    private void OnPreferencesChanged()
    {
        string persistedLanguage = _preferences.PersistedLanguage;
        if (string.Equals(_lastPersistedLanguage, persistedLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _lastPersistedLanguage = persistedLanguage;
        _reasonCache.Invalidate();
        _quotCache.Invalidate();
        _testCatalogCache.Invalidate();
        _techniqueCatalogCache.Invalidate();
        _techniqueCatalogService.Invalidate();
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
