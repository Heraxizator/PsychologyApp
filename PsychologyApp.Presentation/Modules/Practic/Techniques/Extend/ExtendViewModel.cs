using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Extend;

public class ExtendViewModel : BaseViewModel
{
    public ExtendViewModel()
    {

    }

    public ExtendViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "Запасной план";

        Algorithm =
        [
            "1. Назвать проблему",
            "2. Найти альтернативы"
        ];

        Entries =
        [
            new EntryItem
            {
                Title = "Проблема",
                Placeholder = "Проблема"
            },

            new EntryItem
            {
                Title = "Альтернатива 1",
                Placeholder = "Альтернатива 1"
            },

            new EntryItem
            {
                Title = "Альтернатива 2",
                Placeholder = "Альтернатива 2"
            },

            new EntryItem
            {
                Title = "Альтернатива 3",
                Placeholder = "Альтернатива 3"
            },
        ];

        Info = "При стремлении к какой-то цели всегда имейте запасной вариант. А лучше несколько. Ответьте себе на вопрос: «А что я буду делать, если достичь этого не получится?» Зная альтернативы, важность будет уже не такой зашкаливающей.";

        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);

        Navigation = navigation;
    }
}
