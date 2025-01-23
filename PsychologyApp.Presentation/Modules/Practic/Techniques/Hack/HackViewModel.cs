

using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.TechniqueViewModels
{
    public class HackViewModel : BaseViewModel
    {
        public HackViewModel()
        {

        }
        public HackViewModel(INavigation navigation)
        {
            this.Title = "Техника";
            this.Navigation = navigation;
            this.Theory = new Command(ToTheory);
            this.Info = "Если у вас есть тревожащие мысли и воспоминания, которые несут в себе деструктивизм, нужно представить их в виде фото или картины и делать этот образ всё светлее и светлее, пока он совсем не засветится. Если вы хотите что-либо забыть, просто делайте картинку светлой до тех пор, пока не перестанете видеть, что на ней изображено.";
        }
    }
}
;