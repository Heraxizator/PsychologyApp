using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public class DialogService(IPageHost pageHost) : IDialogService
{
    public async Task ShowAsync(string title, string message)
    {
        Page page = RequireActivePage();
        var popup = new AppDialogPopup(title, message, AppStrings.Ok, cancel: null);
        await page.ShowPopupAsync(popup, CreateDialogOptions(popup));
    }

    public async Task<bool> AskAsync(string title, string message, string accept, string cancel)
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
