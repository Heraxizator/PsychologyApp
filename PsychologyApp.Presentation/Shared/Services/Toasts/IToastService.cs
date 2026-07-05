using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Toasts;

public interface IToastService
{
    void LongToast(string message, AppToastKind kind = AppToastKind.Info);

    void ShortToast(string message, AppToastKind kind = AppToastKind.Info);
}
