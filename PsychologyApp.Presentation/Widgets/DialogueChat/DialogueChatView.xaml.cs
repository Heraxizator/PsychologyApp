using System.Collections.Specialized;

namespace PsychologyApp.Presentation.Widgets.DialogueChat;

public partial class DialogueChatView : ContentView
{
    private ChatViewModelBase? _viewModel;

    public DialogueChatView()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (_viewModel is not null)
        {
            _viewModel.Messages.CollectionChanged -= OnMessagesChanged;
        }

        _viewModel = BindingContext as ChatViewModelBase;
        if (_viewModel is not null)
        {
            _viewModel.Messages.CollectionChanged += OnMessagesChanged;
        }
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int last = (_viewModel?.Messages.Count ?? 0) - 1;
        if (last >= 0)
        {
            MessageList.ScrollTo(last, position: ScrollToPosition.End, animate: true);
        }
    }
}
