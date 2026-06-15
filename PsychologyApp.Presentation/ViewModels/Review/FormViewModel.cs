using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Review;

public enum FeedbackChannel
{
    Email,
    Sms,
    Share
}

public partial class FormViewModel : BaseViewModel
{
    public const string FeedbackEmailSubject = "Psychology App feedback";

    public ICommand Send { get; private set; } = default!;

    private readonly IDialogService _dialogService;
    private readonly AppSettings _settings;

    public FormViewModel(IDialogService dialogService, IOptions<AppSettings> settings)
    {
        _dialogService = dialogService;
        _settings = settings.Value;
        ModuleName = AppStrings.ReviewTitle;
        PageName = AppStrings.ReviewPage;
        MessageText = string.Empty;

        Send = new AsyncCommand(SendAsync);
    }

    private string message_text = string.Empty;
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
