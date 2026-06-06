using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.Settings;

public class SettingsViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    public SettingsViewModel(INavigation navigation, IDialogService dialogService, INavigationService navigationService)
    {
        _dialogService = dialogService;
        _navigationService = navigationService;
        ModuleName = "Практик";
        PageName = "Настройки";

        BindNavigation(navigation);
        LoadFromPreferences();

        Finish = new AsyncCommand(ToEndAsync);
    }

    private void LoadFromPreferences()
    {
        UserPreferencesState state = UserPreferences.Load();
        Theme = state.Theme;
        Color = state.Color;
        Form = state.Form;
        Size = state.Size;
        IsThick = state.IsBold;
    }

    private async Task ToEndAsync()
    {
        UserPreferences.Save(new UserPreferencesState
        {
            Theme = Theme,
            Color = Color,
            Form = Form,
            Size = Size,
            IsBold = IsThick
        });

        await _dialogService.ShowAsync("Информация", "Настройки будут применены при следующем запуске приложения");
        await _navigationService.GoBackAsync();
    }

    public string theme { get; private set; } = UserPreferences.DefaultTheme;
    public string Theme
    {
        get => theme;
        set
        {
            if (theme != value)
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
    }

    public string color { get; private set; } = UserPreferences.DefaultColor;
    public string Color
    {
        get => color;
        set
        {
            if (color != value)
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public string form { get; private set; } = UserPreferences.DefaultForm;
    public string Form
    {
        get => form;
        set
        {
            if (form != value)
            {
                form = value;
                OnPropertyChanged(nameof(Form));
            }
        }
    }

    public string size { get; private set; } = UserPreferences.DefaultSize;
    public string Size
    {
        get => size;
        set
        {
            if (size != value)
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }
    }

    public bool isThick;
    public bool IsThick
    {
        get => isThick;
        set
        {
            if (isThick != value)
            {
                isThick = value;
                OnPropertyChanged(nameof(IsThick));
            }
        }
    }
}
