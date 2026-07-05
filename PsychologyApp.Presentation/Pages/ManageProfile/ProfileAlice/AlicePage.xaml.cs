using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileAlice;

public partial class AlicePage : ContentPage
{
    private const string AllowedAliceHost = "alice.yandex.ru";

    public AlicePage(IAliceViewModelFactory aliceViewModelFactory)
    {
        InitializeComponent();
        BindingContext = aliceViewModelFactory.Create(this);
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (!IsAllowedAliceUrl(e.Url))
        {
            e.Cancel = true;
            return;
        }

        if (BindingContext is AliceViewModel viewModel)
        {
            viewModel.SetLoading(true);
        }
    }

    private void OnWebViewNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (BindingContext is AliceViewModel viewModel)
        {
            viewModel.SetLoading(false);
        }
    }

    private static bool IsAllowedAliceUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return uri.Host.Equals(AllowedAliceHost, StringComparison.OrdinalIgnoreCase);
    }
}
