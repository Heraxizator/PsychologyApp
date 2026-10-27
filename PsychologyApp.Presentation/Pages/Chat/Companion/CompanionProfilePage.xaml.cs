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
        _viewModel.PromptAsync = (title, message, accept, cancel, initial) =>
            DisplayPromptAsync(title, message, accept, cancel, initialValue: initial, maxLength: 30);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
