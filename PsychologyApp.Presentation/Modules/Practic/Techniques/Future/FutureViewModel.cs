using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Future;

public class FutureViewModel : BaseViewModel
{
    public FutureViewModel()
    {

    }

    public FutureViewModel(INavigation navigation)
    {
        ModuleName = "Практик";
        PageName = "50 лет спустя";

        Algorithm =
        [
            "1. Сформулировать проблему",
            "2. Оценить её важность спустя 50 лет"
        ];

        Entries =
        [
            new EntryItem
            {
                Title = "Проблема",
                Placeholder = "Меня уволили"
            },
        ];

        Info = "У нашей психики есть одно замечательное свойство: чем меньше и дальше объект, тем меньше его значение. Если что-то для нас не важно, то это нас не беспокоит. Маленькая точка всегда меньше привлекает, чем что-то крупная. Поэтому одним из верных способов решить эмоцинальную проблему является механическое понижение её важности с помощью воображаемого отдаления и уменьшения. Всё просто до безобразия!";
        Navigation = navigation;
    }

}
