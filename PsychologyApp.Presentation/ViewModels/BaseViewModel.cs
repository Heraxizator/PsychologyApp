using MobileHelperMaui.Services.Implementations;
using MobileHelperMaui.Services.Interfaces;
using MobileHelperMaui.Views.TechniquePages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace PsychologyApp.Presentation.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDialog DialogService = new DialogService();
        public INavigation? Navigation { get; set; }
        public ICommand? Finish { get; set; }
        public ICommand? Theory { get; set; }
        public ICommand? Cancel { get; set; }
        public ICommand? Reload { get; set; }
        public ICommand? Start { get; set; }
        public string? Info { get; set; }

        private string title = string.Empty;
        public string Title
        {
            get => this.title;
            set => SetProperty(ref this.title, value);
        }

        public async void ToTheory(object obj)
        {
            await this.Navigation.PushAsync(new TheoryPage(this.Info), false);
        }

        public async void ToFinish(object obj)
        {
            _ = await this.Navigation.PopAsync(false);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed == null)
            {
                return;
            }

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Public Properties

        protected bool _fail_visibility;
        public bool IsFail
        {
            get => this._fail_visibility;
            set
            {
                if (this._fail_visibility != value)
                {
                    this._fail_visibility = value;
                    OnPropertyChanged(nameof(IsFail));
                }
            }
        }

        private bool _init_visibility;
        public bool IsInit
        {
            get => this._init_visibility;
            set
            {
                if (this._init_visibility != value)
                {
                    this._init_visibility = value;
                    OnPropertyChanged(nameof(IsInit));
                }
            }
        }

        protected bool _done_visibility;
        public bool IsDone
        {
            get => this._done_visibility;
            set
            {
                if (this._done_visibility != value)
                {
                    this._done_visibility = value;
                    OnPropertyChanged(nameof(IsDone));
                }
            }
        }

        protected string _progress_text;
        public string ProgressText
        {
            get => this._progress_text;
            set
            {
                if (this._progress_text != value)
                {
                    this._progress_text = value;
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        protected bool _created_visibility;
        public bool IsCreated
        {
            get => this._created_visibility;
            set
            {
                if (this._created_visibility != value)
                {
                    this._created_visibility = value;
                    OnPropertyChanged(nameof(IsCreated));
                }
            }
        }

        #endregion

        protected void SetDefault()
        {
            this.IsInit = false;
            this.IsFail = false;
            this.IsDone = false;
            this.IsCreated = false;
        }

        public void SetDone()
        {
            SetDefault();

            this.IsDone = true;
        }

        public void SetInit()
        {
            SetDefault();

            this.IsInit = true;
        }

        public void SetFail()
        {
            SetDefault();

            this.IsFail = true;
        }

        public void SetCreated()
        {
            SetDefault();

            this.IsCreated = true;
        }
    }
}
