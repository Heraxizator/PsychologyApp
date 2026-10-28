using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

public partial class CompanionProfilePage : ContentPage
{
    private readonly CompanionProfileViewModel _viewModel;

    public CompanionProfilePage(IChatViewModelFactory viewModelFactory, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateProfile(hostNavigation);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
