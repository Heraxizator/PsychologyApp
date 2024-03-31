using AutoMapper;
using MobileHelperMaui.Services;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Repositories;
using PsychologyApp.Infrastructure.Share;
using PsychologyApp.Infrastructure.Uow;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace MobileHelper.ViewModels.ConstructorViewModels
{
    public class DesignerViewModel : BaseViewModel
    {
        private TechniqueService _service;

        public ICommand ExecuteTechnique { get; set; }
        public ICommand OpenCamera { get; set; }
        public ICommand OpenGallery { get; set; }
        private string? _name_string { get; set; }
        private string? _describtion_string { get; set; }
        private string? _theme_string { get; set; }
        private string? _author_string { get; set; }
        private string? _algorithm_string { get; set; }
        private string? _path_string { get; set; }
        private string? _aim_string { get; set; }
        private int currentId { get; set; }

        public DesignerViewModel() { }

        [Obsolete]
        public DesignerViewModel(INavigation navigation, int id)
        {
            this.currentId = id;

            this.Path = "technique.png";

            this.Title = "Конструктор";

            this.Navigation = navigation;

            InitService();

            this.OpenCamera = new Command(ToOpenCamera);

            this.OpenGallery = new Command(ToOpenGallery);

            InitAsync();
        }

        [Obsolete]
        private async void InitAsync()
        {
            if (this.currentId != -1)
            {
                TechniqueDTO current_item = await _service.GetTechniqueById(this.currentId);

                this.Aim = "Изменить";

                this.Name = current_item.Header;

                this.Description = current_item.Describtion;

                this.Theme = current_item.Subject;

                this.Author = current_item.Author;

                this.Algorithm = current_item.Algorithm;

                this.Path = current_item.Image;

                this.ExecuteTechnique = new Command(ToChangeTechnique);
            }

            else
            {
                this.Aim = "Добавить";

                this.ExecuteTechnique = new Command(ToAddTechnique);
            }
        }

        private void InitService()
        {
            ApplicationDbContext context = new();

            UnitOfWork unitOfWork = new(context);

            GenericRepository<Technique> repository = new(context);

            MapperConfiguration configuration = new(cfg => {
                cfg.CreateMap<TechniqueDTO, Technique>();
                cfg.CreateMap<Technique, TechniqueDTO>();
            });

            Mapper mapper = new(configuration);

            this._service = new TechniqueService(repository, unitOfWork, mapper);
        }

        private async void ToOpenCamera(object obj)
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                this.DialogService.ShowAsync("Ошибка", "Камера не поддерживается на вашем устройстве");
                return;
            }

            try
            {
                FileResult? photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    this.Path = photo.FullPath;
                }
            }

            catch (FeatureNotSupportedException)
            {
                this.DialogService.ShowAsync("Ошибка", "Камера не поддерживается на вашем устройстве");
            }
            catch (PermissionException)
            {
                this.DialogService.ShowAsync("Ошибка", "Приложению не предоставлено разрешение на использование камеры");
            }
            catch (Exception)
            {
                this.DialogService.ShowAsync("Ошибка", "Не удалось применить камеру. Напишите в техническую поддержку");
            }
        }

        private async void ToOpenGallery(object obj)
        {
            try
            {
                FileResult? photo = await MediaPicker.PickPhotoAsync();

                if (photo != null)
                {
                    this.Path = photo.FullPath;
                }
            }

            catch (FeatureNotSupportedException)
            {
                this.DialogService.ShowAsync("Ошибка", "Галерея не поддерживается на вашем устройстве");
            }
            catch (PermissionException)
            {
                this.DialogService.ShowAsync("Ошибка", "Приложению не предоставлено разрешение на использование галереи");
            }
            catch (Exception)
            {
                this.DialogService.ShowAsync("Ошибка", "Не удалось применить галерею. Напишите в техническую поддержку");
            }
        }

        [Obsolete]
        private async void ToChangeTechnique(object obj)
        {
            TechniqueDTO item = new()
            {
                Id = this.currentId,
                Header = this.Name,
                Describtion = this.Description,
                Subject = this.Theme,
                Author = this.Author,
                Algorithm = this.Algorithm,
                Image = this.Path
            };

            await this._service.UpdateTechnique(item);

            try
            {
                MessagingCenter.Send<object, TechniqueDTO>(this, "change", item);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _ = this.Navigation.PopToRootAsync(false);
            
        }

        [Obsolete]
        private async void ToAddTechnique(object obj)
        {
            if (!string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.Description) && !string.IsNullOrEmpty(this.Theme)
                && !string.IsNullOrEmpty(this.Author) && !string.IsNullOrEmpty(this.Algorithm))
            {
                string date = DateTime.Now.ToString().Split(' ').First();

                TechniqueBuilder builder = new();

                TechniqueDTO technique = builder
                    .SetIdentifier(-1)
                    .SetTitle(this.Name)
                    .SetSubtitle(this.Description)
                    .SetTheme(this.Theme)
                    .SetImage(this.Path)
                    .SetAuthor(this.Author)
                    .SetAlgorithm(this.Algorithm)
                    .SetDate(date)
                    .Build();

                await this._service.AddNewTechnique(technique);

                MessagingCenter.Send<object, TechniqueDTO>(this, "add", technique);

                await this.Navigation.PopAsync(false);
            }

            else
            {
                this.DialogService.ShowAsync("Ошибка", "Необходимо заполнить все поля");
            }
        }

        #region Fluent Builder
        public class TechniqueBuilder
        {
            private readonly TechniqueDTO techniqueDB;

            public TechniqueBuilder()
            {
                this.techniqueDB = new TechniqueDTO();
            }

            public TechniqueBuilder SetIdentifier(int identifier)
            {
                this.techniqueDB.Id = identifier;
                return this;
            }

            public TechniqueBuilder SetDate(string date)
            {
                this.techniqueDB.Date = date;
                return this;
            }
            public TechniqueBuilder SetTitle(string title)
            {
                this.techniqueDB.Header = title;
                return this;
            }
            public TechniqueBuilder SetSubtitle(string subtitle)
            {
                this.techniqueDB.Describtion = subtitle;
                return this;
            }

            public TechniqueBuilder SetTheme(string theme)
            {
                this.techniqueDB.Subject = theme;
                return this;
            }

            public TechniqueBuilder SetAuthor(string author)
            {
                this.techniqueDB.Author = author;
                return this;
            }

            public TechniqueBuilder SetAlgorithm(string algorithm)
            {
                this.techniqueDB.Algorithm = algorithm;
                return this;
            }

            public TechniqueBuilder SetImage(string image)
            {
                this.techniqueDB.Image = image;
                return this;
            }

            public TechniqueDTO Build()
            {
                return this.techniqueDB;
            }
        }

        #endregion

        #region Public Properties

        public string? Name
        {
            get => this._name_string;
            set
            {
                if (this._name_string != value)
                {
                    this._name_string = value;
                    OnPropertyChanged(nameof(this.Name));
                }
            }
        }

        public string? Description
        {
            get => this._describtion_string;
            set
            {
                if (this._describtion_string != value)
                {
                    this._describtion_string = value;
                    OnPropertyChanged(nameof(this.Description));
                }
            }
        }

        public string? Theme
        {
            get => this._theme_string;
            set
            {
                if (this._theme_string != value)
                {
                    this._theme_string = value;
                    OnPropertyChanged(nameof(this.Theme));
                }
            }
        }

        public string? Author
        {
            get => this._author_string;
            set
            {
                if (this._author_string != value)
                {
                    this._author_string = value;
                    OnPropertyChanged(nameof(this.Author));
                }
            }
        }

        public string? Algorithm
        {
            get => this._algorithm_string;
            set
            {
                if (this._author_string != value)
                {
                    this._algorithm_string = value;
                    OnPropertyChanged(nameof(this.Algorithm));
                }
            }
        }

        public string? Path
        {
            get => this._path_string;
            set
            {
                if (this._path_string != value)
                {
                    this._path_string = value;
                    OnPropertyChanged(nameof(this.Path));
                }
            }
        }

        public string? Aim
        {
            get => this._aim_string;
            set
            {
                if (this._aim_string != value)
                {
                    this._aim_string = value;
                    OnPropertyChanged(nameof(this.Aim));
                }
            }
        }

        #endregion
    }
}
