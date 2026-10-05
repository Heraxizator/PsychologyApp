namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Where the embedded Alice page may navigate. Alice is Yandex's service and signing in goes through Yandex's own passport pages
/// (passport.yandex.ru), so the whole Yandex family over HTTPS is allowed; anything else (an advertised link, a redirect to a third party)
/// is cancelled and never loads inside the app.
/// </summary>
public static class AliceUrlPolicy
{
    private static readonly string[] AllowedDomains = ["yandex.ru", "yandex.com", "ya.ru"];

    public static bool IsAllowed(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        string host = uri.Host.ToLowerInvariant();
        return AllowedDomains.Any(domain => host == domain || host.EndsWith("." + domain, StringComparison.Ordinal));
    }
}
