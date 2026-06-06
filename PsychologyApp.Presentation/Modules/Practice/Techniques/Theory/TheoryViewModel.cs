using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.TechniqueViewModels;

public class TheoryViewModel : BaseViewModel
{
    private string text { get; set; } = default!;

    public string PageTitle => AppStrings.TechniqueTheory;
    public string BackText => AppStrings.Back;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigation navigation, string content)
    {
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.TechniqueTheory;

        BindNavigation(navigation);
        Text = content;
        UserPreferences.Changed += OnPreferencesChanged;
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(BackText));
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
