using System.Collections.Specialized;
using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Conversation;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;

    public ChatPage(IChatViewModelFactory viewModelFactory, long? sessionId, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateConversation(sessionId, hostNavigation);
        BindingContext = _viewModel;
        _viewModel.Messages.CollectionChanged += OnMessagesChanged;

        // Unloaded (page popped), not Disappearing: the latter also fires when the app is merely backgrounded.
        Unloaded += (_, _) => _viewModel.Close();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearedAsync().FireAndForget();
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int last = _viewModel.Messages.Count - 1;
        if (last >= 0)
        {
            MessageList.ScrollTo(last, position: ScrollToPosition.End, animate: e.Action == NotifyCollectionChangedAction.Add && last > 0);
        }
    }
}
