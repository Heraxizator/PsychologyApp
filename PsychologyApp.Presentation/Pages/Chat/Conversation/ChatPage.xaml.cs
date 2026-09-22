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

    /// <summary>Whether the bottom of the conversation was on screen at the last scroll event; new messages only pull the view down while this is true.</summary>
    private bool _isNearBottom = true;

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
        AppearAsync().FireAndForget();
    }

    private async Task AppearAsync()
    {
        await _viewModel.OnAppearedAsync();

        // Belt and suspenders: the CollectionChanged-driven scroll above should already have landed at the bottom,
        // but this settles it once more now that the list is guaranteed laid out, in case that first attempt was
        // too early on a slower device.
        ScrollToBottom(animate: false);
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

    /// <summary>
    /// Follows new messages to the bottom, but only while the person was already there: reading back through history is
    /// not interrupted by the companion "typing" a new reply. Sending a message of one's own always follows, and always
    /// wins. Scrolling is deferred a tick and wrapped defensively: asking Android's CollectionView to scroll to an index
    /// right after a bulk change (loading history fires one Reset for the whole batch) can hit the list before it has
    /// finished laying out the new items and crash the native view.
    /// </summary>
    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_viewModel.Messages.Count == 0)
        {
            return;
        }

        bool isOwnMessage = e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems is { Count: > 0 } added
            && added[^1] is ChatBubble { IsUser: true };
        if (e.Action == NotifyCollectionChangedAction.Add && !_isNearBottom && !isOwnMessage)
        {
            return;
        }

        bool animate = e.Action == NotifyCollectionChangedAction.Add && _viewModel.Messages.Count > 1;
        Dispatcher.Dispatch(() => ScrollToBottom(animate));
    }

    private void OnMessageListScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        int count = _viewModel.Messages.Count;
        bool canScrollUp = count > 0 && e.FirstVisibleItemIndex > 0;
        bool canScrollDown = count > 0 && e.LastVisibleItemIndex >= 0 && e.LastVisibleItemIndex < count - 1;
        ScrollToTopButton.IsVisible = canScrollUp;
        ScrollToBottomButton.IsVisible = canScrollDown;
        _isNearBottom = !canScrollDown;
    }

    private void OnScrollToTopTapped(object? sender, TappedEventArgs e)
    {
        _isNearBottom = false;
        JumpTo(0, ScrollToPosition.Start, sender);
    }

    private void OnScrollToBottomTapped(object? sender, TappedEventArgs e)
    {
        _isNearBottom = true;
        JumpTo(_viewModel.Messages.Count - 1, ScrollToPosition.End, sender);
    }

    private void JumpTo(int index, ScrollToPosition position, object? button)
    {
        if (index < 0)
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

        if (button is VisualElement view)
        {
            UiAnimations.SafePulseAsync(view).FireAndForget();
        }

        SafeScrollTo(index, position, animate: true);
    }

    private void ScrollToBottom(bool animate) => SafeScrollTo(_viewModel.Messages.Count - 1, ScrollToPosition.End, animate);

    private void SafeScrollTo(int index, ScrollToPosition position, bool animate)
    {
        int last = _viewModel.Messages.Count - 1;
        if (last < 0)
        {
            return;
        }

        try
        {
            MessageList.ScrollTo(Math.Clamp(index, 0, last), position: position, animate: animate);
        }
        catch (Exception)
        {
            // A scroll that Android's list isn't ready for must never crash the chat; the message is safely on screen either way.
        }
    }
}
