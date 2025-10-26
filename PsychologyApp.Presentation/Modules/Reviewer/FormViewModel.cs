using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.ReviewViewModels;

public class FormViewModel : BaseViewModel
{
    public ICommand Send { get; private set; } = default!;

    private const string recipient_number = "89142107907";

    public FormViewModel()
    {
        ModuleName = "Отзовик";
        PageName = "Отзыв";

        Send = new Command(async () =>
        {
            if (string.IsNullOrEmpty(MessageText))
            {
                return;
            }

            await SendSms(MessageText, recipient_number);
        });
    }

    public async Task SendSms(string messageText, string recipient)
    {
        try
        {
            SmsMessage message = new(messageText, new[] { recipient });
            await Sms.Default.ComposeAsync(message);
        }

        catch (FeatureNotSupportedException)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync(null, "Отправка СМС не поддерживается");
        }

        catch (Exception)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync(null, "Ошибка при отправке СМС");
        }
    }

    private string message_text = default!;
    public string MessageText
    {
        get => message_text;
        set
        {
            if (message_text != value)
            {
                message_text = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }
    }
}
