using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.TechniqueViewModels;

public class TheoryViewModel : BaseViewModel
{
    private string text { get; set; } = default!;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigation navigation, string content)
    {
        ModuleName = "Практик";
        PageName = "Теория";

        Text = content;
    }

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
}
