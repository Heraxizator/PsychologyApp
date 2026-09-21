using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.ChatList;

public partial class ChatListPage : ContentPage
{
    private const int MaxStaggeredRows = 8;
    private const int RowStaggerMs = 45;

    private readonly ChatListViewModel _viewModel;
    private readonly HashSet<long> _revealed = [];

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

    /// <summary>Rows slide in one after another the first time they are shown. Rows already shown (recycled while scrolling, or rebuilt on refresh) stay put.</summary>
    private void OnChatRowContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement row)
        {
            return;
        }

        if (row.BindingContext is not ChatListItem item || !_revealed.Add(item.Id) || ReduceMotion.IsEnabled)
        {
            UiAnimations.ResetVisualState(row);
            return;
        }

        row.Opacity = 0;
        int index = Math.Clamp(_viewModel.Chats.IndexOf(item), 0, MaxStaggeredRows);
        UiAnimations.SafeRevealLiteAsync(row, allowHidden: true, delayMs: 16 + (index * RowStaggerMs)).FireAndForget();
    }
}
