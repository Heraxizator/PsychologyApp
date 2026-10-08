using System.Collections.Specialized;

namespace PsychologyApp.Presentation.Widgets.DialogueChat;

public partial class DialogueChatView : ContentView
{
    private ChatViewModelBase? _viewModel;

    public DialogueChatView()
    {
        InitializeComponent();
        InputBar.Input.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck | KeyboardFlags.Suggestions);
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

    /// <summary>The slider answers the 0..10 question with the same command the chips used.</summary>
    private void OnTensionChosen(object? sender, int value) =>
        _viewModel?.RatingItems.FirstOrDefault(item => item.Value == value)?.Command.Execute(value);

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int last = (_viewModel?.Messages.Count ?? 0) - 1;
        if (last >= 0)
        {
            MessageList.ScrollTo(last, position: ScrollToPosition.End, animate: true);
        }
    }
}
