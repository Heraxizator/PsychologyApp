using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.ChatList;

public partial class ChatListPage : ContentPage
{
    private readonly ChatListViewModel _viewModel;

    public ChatListPage(IChatViewModelFactory viewModelFactory, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateList(hostNavigation);
        _viewModel.PromptAsync = (title, message, accept, cancel, initial) =>
            DisplayPromptAsync(title, message, accept, cancel, initialValue: initial, maxLength: 60);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }
}
