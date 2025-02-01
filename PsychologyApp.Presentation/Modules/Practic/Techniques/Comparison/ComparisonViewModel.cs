using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Comparison;

public class ComparisonViewModel : BaseViewModel
{
    public ComparisonViewModel()
    {

    }

    public ComparisonViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Сравнение важностей";

        Info = "Основной причиной любого беспокойства является завышенная важность. Поэтому её нужно уметь понижать. Сравнение важностей - один из способов уменьшить значимость того, что тревожит. По сути важность - сопутствуещее любого негатива, включая страх, неверие в себя, завышенную планку, сомнения и другое. Для выполнения этой техники достаточно сравнить то, что было важно в прошлом, важно в настоящем и будет важно в будущем относительно проблемы.";

        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);
        Navigation = navigation;
    }
}
