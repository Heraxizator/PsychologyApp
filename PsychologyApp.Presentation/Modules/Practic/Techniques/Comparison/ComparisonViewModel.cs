using PsychologyApp.Presentation.Templates;
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

        Algorithm =
        [
            "1. Назвать проблему",
            "2. Определить то, что важно в прошлом",
            "3. Определить то, что важно в настоящем",
            "4. Определить то, что важно в будущем",
            "5. Сравнить то, что важно в трёх временах"
        ];

        Entries =
        [
             new EntryItem
            {
                Title = "Проблема",
                Placeholder = "У меня нет работы"
            },

            new EntryItem
            {
                Title = "Прошлое",
                Placeholder = "Хорошо учиться"
            },

            new EntryItem
            {
                Title = "Настоящее",
                Placeholder = "Найти работу"
            },

            new EntryItem
            {
                Title = "Прошлое",
                Placeholder = "Сохранить "
            },
        ];

        Info = "Основной причиной любого беспокойства является завышенная важность. Поэтому её нужно уметь понижать. Сравнение важностей - один из способов уменьшить значимость того, что тревожит. По сути важность - сопутствуещее любого негатива, включая страх, неверие в себя, завышенную планку, сомнения и другое. Для выполнения этой техники достаточно сравнить то, что было важно в прошлом, важно в настоящем и будет важно в будущем относительно проблемы.";

        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);
        Navigation = navigation;
    }
}
