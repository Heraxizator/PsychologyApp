using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public class DialogService(IPageHost pageHost) : IDialogService
{
    // One dialog or sheet at a time: a double tap on the button that opens one would otherwise stack two identical popups.
    private int _isShowing;

    public async Task ShowAsync(string? title, string message)
    {
        if (!TryEnter())
        {
            return;
        }

        try
        {
            Page page = RequireActivePage();
            var popup = new AppDialogPopup(title, message, AppStrings.Ok, cancel: null);
            await page.ShowPopupAsync(popup, CreateDialogOptions(popup));
        }
        finally
        {
            Leave();
        }
    }

    public async Task<bool> AskAsync(string? title, string message, string accept, string cancel)
    {
        // A question that is already on screen: this duplicate answers "no", the safe default for a confirmation.
        if (!TryEnter())
        {
            return false;
        }

        try
        {
            Page page = RequireActivePage();
            var popup = new AppDialogPopup(title, message, accept, cancel);
            IPopupResult popupResult = await page.ShowPopupAsync(popup, CreateDialogOptions(popup));
            if (popupResult.WasDismissedByTappingOutsideOfPopup)
            {
                return false;
            }

            return popup.DialogResult == true;
        }
        finally
        {
            Leave();
        }
    }

    public async Task<string?> PromptPasswordAsync(string? title, string message, string placeholder, string accept, string cancel)
    {
        if (!TryEnter())
        {
            return null;
        }

        try
        {
            Page page = RequireActivePage();
            var popup = new AppDialogPopup(title, message, accept, cancel, placeholder);
            IPopupResult popupResult = await page.ShowPopupAsync(popup, CreateDialogOptions(popup));
            if (popupResult.WasDismissedByTappingOutsideOfPopup || popup.DialogResult != true)
            {
                return null;
            }

            return popup.InputText;
        }
        finally
        {
            Leave();
        }
    }

    public async Task<string?> PickOptionAsync(string title, IReadOnlyList<string> options, string cancel)
    {
        if (!TryEnter())
        {
            return null;
        }

        try
        {
            Page page = RequireActivePage();
            var popup = new AppOptionsSheetPopup(title, options, cancel);
            await page.ShowPopupAsync(popup, AppPopupOptions.OptionsSheet);
            return popup.SelectedOption;
        }
        finally
        {
            Leave();
        }
    }

    private bool TryEnter() => Interlocked.CompareExchange(ref _isShowing, 1, 0) == 0;

    private void Leave() => Volatile.Write(ref _isShowing, 0);

    private static PopupOptions CreateDialogOptions(AppDialogPopup popup) =>
        new()
        {
            PageOverlayColor = AppPopupOptions.Dialog.PageOverlayColor,
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shape = null,
            Shadow = null,
            OnTappingOutsideOfPopup = popup.MarkDismissedOutside,
        };

    private Page RequireActivePage() =>
        pageHost.GetActivePage()
        ?? throw new InvalidOperationException("No active page available for dialog.");
}
