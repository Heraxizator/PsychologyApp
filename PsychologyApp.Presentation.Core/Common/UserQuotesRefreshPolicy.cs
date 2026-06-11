namespace PsychologyApp.Presentation.Common;

public static class UserQuotesRefreshPolicy
{
    public static bool ShouldReload(bool quotesLoadedOnce, bool forceReload) =>
        forceReload || !quotesLoadedOnce;
}
