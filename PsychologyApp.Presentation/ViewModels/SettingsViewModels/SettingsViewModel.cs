using PsychologyApp.Presentation.ServiceLocator;
using PsychologyApp.Presentation.ServiceLocator.Dialog;
using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.SettingsViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public string theme { get; set; }
        public string color { get; set; }
        public string form { get; set; }
        public string size { get; set; }
        public bool isThick { get; set; }
        public SettingsViewModel()
        {

        }
        public SettingsViewModel(INavigation navigation)
        {
            this.Title = "Настройки";
            this.Navigation = navigation;
            this.Finish = new Command(ToEnd);
        }

        private void ToEnd(object obj)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Предупреждение", "Изменения будут применены при следующем запуске приложения");

            Preferences.Set("Theme", this.Theme);
            Preferences.Set("Color", this.Color);
            Preferences.Set("Form", this.Form);
            Preferences.Set("Size", this.Size);
            Preferences.Set("IsBold", this.IsThick);

        }

        public string Theme
        {
            get => this.theme;
            set
            {
                if (this.theme != value)
                {
                    this.theme = value;
                    OnPropertyChanged(nameof(this.Theme));
                }
            }
        }

        public string Color
        {
            get => this.color;
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    OnPropertyChanged(nameof(this.Color));
                }
            }
        }

        public string Form
        {
            get => this.form;
            set
            {
                if (this.form != value)
                {
                    this.form = value;
                    OnPropertyChanged(nameof(this.Form));
                }
            }
        }

        public string Size
        {
            get => this.size;
            set
            {
                if (this.size != value)
                {
                    this.size = value;
                    OnPropertyChanged(nameof(this.Size));
                }
            }
        }

        public bool IsThick
        {
            get => this.isThick;
            set
            {
                if (this.isThick != value)
                {
                    this.isThick = value;
                    OnPropertyChanged(nameof(this.IsThick));
                }
            }
        }
    }
}
