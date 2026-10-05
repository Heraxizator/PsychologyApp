using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Shared.UI.Overlays;

public partial class AppDialogPopup : Popup
{
    private bool _isClosing;

    public AppDialogPopup(string? title, string message, string accept, string? cancel, string? passwordPlaceholder = null)
    {
        InitializeComponent();

        if (passwordPlaceholder is not null)
        {
            PasswordEntry.IsVisible = true;
            PasswordEntry.Placeholder = passwordPlaceholder;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            TitleLabel.IsVisible = false;
        }
        else
        {
            TitleLabel.Text = title;
        }

        MessageLabel.Text = message;
        AcceptLabel.Text = accept;

        if (cancel is null)
        {
            CancelTapArea.IsVisible = false;
            ActionDivider.IsVisible = false;
            Grid.SetColumn(AcceptTapArea, 0);
            Grid.SetColumnSpan(AcceptTapArea, 3);
        }
        else
        {
            CancelLabel.Text = cancel;
        }

        Loaded += OnLoaded;
    }

    public bool? DialogResult { get; private set; }

    /// <summary>What was typed in the masked field; empty when the dialog has none.</summary>
    public string InputText => PasswordEntry.Text ?? string.Empty;

    public void MarkDismissedOutside() => DialogResult = false;

    private void OnLoaded(object? sender, EventArgs e) =>
        UiAnimations.AnimateAlertPopInAsync(DialogCardHost).FireAndForget();

    private void OnAcceptTapped(object? sender, TappedEventArgs e) =>
        CloseWithResult(true).FireAndForget();

    private void OnCancelTapped(object? sender, TappedEventArgs e) =>
        CloseWithResult(false).FireAndForget();

    private async Task CloseWithResult(bool accepted)
    {
        if (_isClosing)
        {
            return;
        }

        _isClosing = true;
        DialogResult = accepted;
        await UiAnimations.AnimateAlertPopOutAsync(DialogCardHost);
        await CloseAsync();
    }
}
