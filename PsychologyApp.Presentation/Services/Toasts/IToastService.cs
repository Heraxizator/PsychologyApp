namespace PsychologyApp.Presentation.Services.Toasts;

public interface IToastService
{
    void LongToast(string message);
    void ShortToast(string message);
}
