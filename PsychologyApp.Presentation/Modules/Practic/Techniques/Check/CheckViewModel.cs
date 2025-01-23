using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.TechniqueViewModels;

public class CheckViewModel : BaseViewModel
{
    public CheckViewModel()
    {

    }

    public CheckViewModel(INavigation navigation)
    {
        Title = "Техника";
        Info = "Известно, что избыток внимания часто ведёт к избытку важности. Попробуйте меньше думать о том, что вас беспокоит. И, возможно, это сработает.";
        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);
        Navigation = navigation;
    }
}
