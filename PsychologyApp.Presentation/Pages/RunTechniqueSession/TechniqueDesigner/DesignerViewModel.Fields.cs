using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

public partial class DesignerViewModel
{
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
            if (_algorithm_string != value)
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

    private void ApplyForm(DesignerTechniqueForm form)
    {
        Name = form.Name;
        Description = form.Description;
        Theme = form.Theme;
        Author = form.Author;
        Actions = form.Actions;
        Path = form.Path;
    }

    private DesignerTechniqueForm CaptureForm() =>
        new(Name, Description, Theme, Author, Actions, Path);
}
