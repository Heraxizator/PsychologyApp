using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Presentation.Services.Tests;

namespace PsychologyApp.Presentation.Common;

public sealed class ContentCacheInvalidator
{
    public ContentCacheInvalidator(
        CachedReasonContentProvider reasonCache,
        CachedQuotContentProvider quotCache,
        CachedTestCatalogService testCatalogCache)
    {
        UserPreferences.Changed += () =>
        {
            reasonCache.Invalidate();
            quotCache.Invalidate();
            testCatalogCache.Invalidate();
        };
    }
}
