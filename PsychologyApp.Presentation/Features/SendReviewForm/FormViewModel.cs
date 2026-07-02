using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

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
    public ICommand BackCommand { get; }

    private readonly IDialogService _dialogService;
    private readonly AppSettings _settings;

    public FormViewModel(IDialogService dialogService, IOptions<AppSettings> settings, INavigationService navigationService)
    {
        _dialogService = dialogService;
        _settings = settings.Value;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
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
