using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.SettingsViewModels;

public class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel() { }

    public SettingsViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Настройки";

        Navigation = navigation;

        Finish = new Command(ToEnd);
    }

    private void ToEnd(object obj)
    {
        ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Предупреждение", "Изменения будут применены при следующем запуске приложения");

        Preferences.Set("Theme", Theme);
        Preferences.Set("Color", Color);
        Preferences.Set("Form", Form);
        Preferences.Set("Size", Size);
        Preferences.Set("IsBold", IsThick);

    }

    public string theme { get; private set; } = default!;
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

    public string color { get; private set; } = default!;
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

    public string form { get; private set; } = default!;
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

    public string size { get; private set; } = default!;
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

    public bool isThick { get; private set; } = default!;
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
