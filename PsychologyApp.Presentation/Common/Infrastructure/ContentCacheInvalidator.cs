using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonService;

namespace PsychologyApp.Presentation.Common;

public sealed class ContentCacheInvalidator
{
    public ContentCacheInvalidator(
        CachedReasonContentProvider reasonCache,
        CachedQuotContentProvider quotCache)
    {
        UserPreferences.Changed += () =>
        {
            reasonCache.Invalidate();
            quotCache.Invalidate();
        };
    }
}
