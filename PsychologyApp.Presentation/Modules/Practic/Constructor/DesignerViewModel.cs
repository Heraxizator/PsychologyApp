using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.Modules.Practic.Messages;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practic.Constructor;

public class DesignerViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly TechniqueService _techniqueService = new();

    public ICommand ExecuteTechnique { get; private set; } = default!;

    public DesignerViewModel() { }

    public DesignerViewModel(INavigation navigation, long techniqueId)
    {
        try
        {
            _techniqueId = techniqueId;

            this.Path = "method.png";

            this.ModuleName = "Практик";
            this.PageName = "Конструктор";

            Navigation = navigation;

            this.Finish = new Command(async () => await navigation.PopAsync(false));

            Task.Run(async () => await InitAsync(navigation, Constants.SmallBaseTimeout));
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void ExecuteOperation(INavigation navigation)
    {
        if (_techniqueId <= 0)
        {
            ToAddTechnique(navigation);
        }

        else
        {
            ToChangeTechnique(navigation);
        }
    }

    private async Task FillAsync(INavigation navigation, int cancelTimeout)
    {
        try
        {
            TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueById(_techniqueId, cancelTimeout);

            if (techniqueDTO.TechniqueId <= 0)
            {
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Name = techniqueDTO.Header;
                Description = techniqueDTO.Describtion;
                Theme = techniqueDTO.Subject;
                Author = techniqueDTO.Author;
                Actions = techniqueDTO.Actions;
                Path = techniqueDTO.Image;
            });
        }
        
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private Task InitAsync(INavigation navigation, int cancelTimeout)
    {
        return FillAsync(navigation, cancelTimeout);
    }

    private async void ToChangeTechnique(INavigation navigation)
    {
        try
        {
            TechniqueDTO item = new()
            {
                TechniqueId = _techniqueId,
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Author = Author,
                Actions = Actions,
                Image = Path,
                Number = Guid.NewGuid().ToString(),
                Date = GetTechniqueDate()
            };

            await _techniqueService.UpdateTechnique(item);

            TechniqueMessenger.Send(new TechniqueMessage
            {
                MessageType = TechniqueMessageType.Change,
                Technique = item
            });

            await navigation.PopToRootAsync(false);
        }

        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private async void ToAddTechnique(INavigation navigation)
    {
        try
        {
            TechniqueDTO technique = new()
            {
                TechniqueId = -1,
                Number = Guid.NewGuid().ToString(),
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Image = Path,
                Author = Author,
                Actions = Actions,
                Date = GetTechniqueDate()
            };

            await _techniqueService.AddNewTechnique(technique);

            TechniqueMessenger.Send(new TechniqueMessage
            {
                MessageType = TechniqueMessageType.Add,
                Technique = technique
            });

            _ = await navigation.PopAsync(false);
        }

        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static string GetTechniqueDate()
    {
        return DateTime.Now.ToString().Split(' ').First();
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
    public string? Actions
    {
        get => _algorithm_string;
        set
        {
            if (_author_string != value)
            {
                _algorithm_string = value;
                OnPropertyChanged(nameof(Actions));
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

    #endregion
}
