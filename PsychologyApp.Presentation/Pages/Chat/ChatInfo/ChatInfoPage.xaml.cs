using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.ChatInfo;

public partial class ChatInfoPage : ContentPage
{
    private readonly ChatInfoViewModel _viewModel;

    public ChatInfoPage(IChatViewModelFactory viewModelFactory, long sessionId, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateInfo(sessionId, hostNavigation);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
