using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Technique.Extend;

public class ExtendViewModel : BaseViewModel
{
    public ExtendViewModel()
    {

    }

    public ExtendViewModel(INavigation navigation)
    {
        Title = "Техника";
        Info = "При стремлении к какой-то цели всегда имейте запасной вариант. А лучше несколько. Ответьте себе на вопрос: «А что я буду делать, если достичь этого не получится?» Зная альтернативы, важность будет уже не такой зашкаливающей.";
        Finish = new Command(ToFinish);
        Theory = new Command(ToTheory);
        Navigation = navigation;
    }
}
