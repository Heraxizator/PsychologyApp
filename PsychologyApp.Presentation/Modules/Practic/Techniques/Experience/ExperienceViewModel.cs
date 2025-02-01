

using PsychologyApp.Presentation.ViewModels;

namespace MobileHelper.ViewModels.TechniqueViewModels;

public class ExperienceViewModel : BaseViewModel
{

    public ExperienceViewModel()
    {

    }

    public ExperienceViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Техника модификации опыта";

        Navigation = navigation;
        Info = "Техника модификации опыта (ТМО) — это эффективный инструмент психологической помощи, позволяющий разобраться как с устоявшимися, так и с недавно появившимися ограничениями, убеждениями, моделями поведения в различных ситуациях. ТМО подходит для самокоучинга. Суть ТМО — позитивное перепроживание ситуаций, давших вам негативный опыт.";
    }

}
