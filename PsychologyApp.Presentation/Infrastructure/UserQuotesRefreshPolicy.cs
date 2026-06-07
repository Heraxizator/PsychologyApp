namespace PsychologyApp.Presentation.Infrastructure;

public static class UserQuotesRefreshPolicy
{
    public static bool ShouldReload(bool quotesLoadedOnce, bool forceReload) =>
        forceReload || !quotesLoadedOnce;
}
