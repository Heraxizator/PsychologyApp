namespace PsychologyApp.Presentation.Shared.Services.Toasts;

public interface IToastService
{
    void LongToast(string message);
    void ShortToast(string message);
}
