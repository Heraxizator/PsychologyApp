using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Check;

public class CheckViewModel : BaseViewModel
{
    public CheckViewModel()
    {

    }

    public CheckViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Проверь Это";

        Algorithm =
        [
            "1. Найти то, что вас беспокоит",
            "2. А стоит ли это вашего внимания?",
            "3. Сделать вывод"
        ];

        Entries = [];

        Info = "Известно, что избыток внимания часто ведёт к избытку важности. Попробуйте меньше думать о том, что вас беспокоит. И, возможно, это сработает.";

        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);

        Navigation = navigation;
    }
}
