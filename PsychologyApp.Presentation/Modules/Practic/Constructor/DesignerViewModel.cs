using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Base.ServiceLocator;
using PsychologyApp.Presentation.Base.ServiceLocator.Dialog;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practic.Constructor;

public class DesignerViewModel : BaseViewModel
{
    private static Task? Initialization = default;
    private readonly TechniqueService _techniqueService = new();

    public ICommand ExecuteTechnique { get; private set; } = default!;
    public ICommand OpenCamera { get; private set; } = default!;
    public ICommand OpenGallery { get; private set; } = default!;

    private readonly long currentId;

    public DesignerViewModel() { }

    [Obsolete]
    public DesignerViewModel(INavigation navigation, long id)
    {
        this.ModuleName = "Практик";
        this.PageName = "Конструктор";

        currentId = id;

        Navigation = navigation;

        Finish = new Command(ToFinish);

        Path = "technique.png";

        OpenCamera = new Command(ToOpenCamera);
        OpenGallery = new Command(ToOpenGallery);

        Initialization = InitAsync(navigation, 10000);
    }

    [Obsolete]
    private async Task InitEditAsync(INavigation navigation, int cancelTimeout)
    {
        TechniqueDTO current_item = await _techniqueService.GetTechniqueById(currentId, cancelTimeout);

        Aim = "Изменить";

        Name = current_item.Header;
        Description = current_item.Describtion;
        Theme = current_item.Subject;
        Author = current_item.Author;
        Algorithm = current_item.Algorithm;
        Path = current_item.Image;

        ExecuteTechnique = new Command(() => ToChangeTechnique(navigation));
    }

    [Obsolete]
    private void InitAdd(INavigation navigation)
    {
        Aim = "Добавить";

        ExecuteTechnique = new Command(() => ToAddTechnique(navigation));
    }

    [Obsolete]
    private async Task InitAsync(INavigation navigation, int cancelTimeout)
    {
        if (currentId == -1)
        {
            InitAdd(navigation);
        }

        else
        {
            await InitEditAsync(navigation, cancelTimeout);
        }
    }

    private async void ToOpenCamera(object obj)
    {
        try
        {
            FileResult? photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                Path = photo.FullPath;
            }
        }

        catch (FeatureNotSupportedException)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Камера не поддерживается на вашем устройстве");
        }

        catch (PermissionException)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Приложению не предоставлено разрешение на использование камеры");
        }

        catch (Exception)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Не удалось применить камеру. Напишите в техническую поддержку");
        }
    }

    private async void ToOpenGallery(object obj)
    {
        try
        {
            FileResult? photo = await MediaPicker.PickPhotoAsync();

            if (photo != null)
            {
                Path = photo.FullPath;
            }
        }

        catch (FeatureNotSupportedException)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Галерея не поддерживается на вашем устройстве");
        }

        catch (PermissionException)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Приложению не предоставлено разрешение на использование галереи");
        }

        catch (Exception)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Не удалось применить галерею. Напишите в техническую поддержку");
        }
    }

    [Obsolete]
    private async void ToChangeTechnique(INavigation navigation)
    {
        try
        {
            TechniqueDTO item = new()
            {
                TechniqueId = currentId,
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Author = Author,
                Algorithm = Algorithm,
                Image = Path
            };

            await _techniqueService.UpdateTechnique(item);

            MessagingCenter.Send<object, TechniqueDTO>(this, "change", item);

            await navigation.PopToRootAsync(false);
        }

        catch (Exception)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Убедитесь, что все поля заполнены");
        }
    }

    [Obsolete]
    private async void ToAddTechnique(INavigation navigation)
    {
        try
        {
            string techniqueDate = DateTime.Now.ToString().Split(' ').First();

            TechniqueDTO technique = new()
            {
                TechniqueId = -1,
                Number = Guid.NewGuid().ToString(),
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Image = Path,
                Author = Author,
                Algorithm = Algorithm,
                Date = techniqueDate
            };

            await _techniqueService.AddNewTechnique(technique);

            MessagingCenter.Send<object, TechniqueDTO>(this, "add", technique);

            _ = await navigation.PopAsync(false);
        }

        catch (Exception)
        {
            ServiceLocator.Instance.GetService<IDialogService>().ShowAsync("Ошибка", "Убедитесь, что все поля заполнены");
        }
    }

    #region Public Properties

    private string? _name_string { get; set; }
    public string? Name
    {
        get => _name_string;
        set
        {
            if (_name_string != value)
            {
                _name_string = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private string? _describtion_string { get; set; }
    public string? Description
    {
        get => _describtion_string;
        set
        {
            if (_describtion_string != value)
            {
                _describtion_string = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    private string? _theme_string { get; set; }
    public string? Theme
    {
        get => _theme_string;
        set
        {
            if (_theme_string != value)
            {
                _theme_string = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
    }

    private string? _author_string { get; set; }
    public string? Author
    {
        get => _author_string;
        set
        {
            if (_author_string != value)
            {
                _author_string = value;
                OnPropertyChanged(nameof(Author));
            }
        }
    }

    private string? _algorithm_string { get; set; }
    public string? Algorithm
    {
        get => _algorithm_string;
        set
        {
            if (_author_string != value)
            {
                _algorithm_string = value;
                OnPropertyChanged(nameof(Algorithm));
            }
        }
    }

    private string? _path_string { get; set; }
    public string? Path
    {
        get => _path_string;
        set
        {
            if (_path_string != value)
            {
                _path_string = value;
                OnPropertyChanged(nameof(Path));
            }
        }
    }

    private string? _aim_string { get; set; }
    public string? Aim
    {
        get => _aim_string;
        set
        {
            if (_aim_string != value)
            {
                _aim_string = value;
                OnPropertyChanged(nameof(Aim));
            }
        }
    }

    #endregion
}
