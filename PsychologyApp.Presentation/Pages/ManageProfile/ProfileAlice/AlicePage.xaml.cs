using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileAlice;

public partial class AlicePage : ContentPage
{
    public AlicePage(IAliceViewModelFactory aliceViewModelFactory)
    {
        InitializeComponent();
        BindingContext = aliceViewModelFactory.Create(this);
    }

    private void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (!AliceUrlPolicy.IsAllowed(e.Url))
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
}
