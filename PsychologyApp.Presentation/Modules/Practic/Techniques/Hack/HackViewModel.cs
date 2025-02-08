using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Hack;

public class HackViewModel : BaseViewModel
{
    public HackViewModel()
    {

    }
    public HackViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Белое Пятно";

        Algorithm =
        [
            "1. Найти то воспоминание, которое беспокоит",
            "2. Представить его как фото",
            "3. Делать его всё светлее и светлее"
        ];

        Entries = [];

        Navigation = navigation;
        Info = "Если у вас есть тревожащие мысли и воспоминания, которые несут в себе деструктивизм, нужно представить их в виде фото или картины и делать этот образ всё светлее и светлее, пока он совсем не засветится. Если вы хотите что-либо забыть, просто делайте картинку светлой до тех пор, пока не перестанете видеть, что на ней изображено.";
    }
}