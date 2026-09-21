using System.Collections.Specialized;
using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Conversation;

public partial class ChatPage : ContentPage
{
    private const double BubbleSlideOffset = 14;
    private const int ChipStaggerMs = 55;
    private const int MaxStaggeredChips = 5;

    private readonly ChatViewModel _viewModel;

    public ChatPage(IChatViewModelFactory viewModelFactory, long? sessionId, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateConversation(sessionId, hostNavigation);
        BindingContext = _viewModel;
        _viewModel.Messages.CollectionChanged += OnMessagesChanged;

        // Real messenger typing: capital letter at the start of each sentence, spell check and suggestions.
        InputEditor.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck | KeyboardFlags.Suggestions);

        // Unloaded (page popped), not Disappearing: the latter also fires when the app is merely backgrounded.
        Unloaded += (_, _) => _viewModel.Close();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearedAsync().FireAndForget();
    }

    /// <summary>A message that has just arrived slides up and fades in once; history and recycled rows are left alone.</summary>
    private void OnBubbleContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement view)
        {
            return;
        }

        if (view.BindingContext is not ChatBubble { Animate: true } bubble || ReduceMotion.IsEnabled)
        {
            UiAnimations.ResetVisualState(view);
            return;
        }

        bubble.Animate = false;
        view.Opacity = 0;
        UiAnimations.SafeRevealLiteAsync(view, y: BubbleSlideOffset, duration: UiAnimations.MediumDuration, allowHidden: true, delayMs: 16).FireAndForget();
    }

    /// <summary>Suggestion chips appear one after another instead of all at once.</summary>
    private void OnChipContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement chip)
        {
            return;
        }

        if (chip.BindingContext is not QuickReplyItem item || ReduceMotion.IsEnabled)
        {
            UiAnimations.ResetVisualState(chip);
            return;
        }

        chip.Opacity = 0;
        int index = Math.Clamp(_viewModel.QuickReplies.IndexOf(item), 0, MaxStaggeredChips);
        UiAnimations.SafeRevealLiteAsync(chip, y: 8, allowHidden: true, delayMs: 60 + (index * ChipStaggerMs)).FireAndForget();
    }

    private void OnSendTapped(object? sender, TappedEventArgs e)
    {
        if (!_viewModel.CanSend)
        {
            return;
        }

        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (FeatureNotSupportedException)
        {
        }

        if (sender is VisualElement button)
        {
            UiAnimations.SafePulseAsync(button).FireAndForget();
        }
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
