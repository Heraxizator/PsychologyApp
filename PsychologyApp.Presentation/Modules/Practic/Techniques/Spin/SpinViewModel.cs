using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.Spin;

public class SpinViewModel : BaseViewModel
{
    public SpinViewModel() { }
    public SpinViewModel(INavigation navigation)
    {
        Navigation = navigation;

        ModuleName = "Практик";
        PageName = "Крутилка";

        Algorithm =
        [
            "1. Найти эпизод, который беспокоит",
            "2. Определить ключевое чувство",
            "3. Оценить чувство по 10-балльной шкале"
        ];

        Entries =
        [
            new EntryItem 
            {
                Title = "Эпизод",
                Placeholder = "Я потерял друга"
            },

            new EntryItem {
                Title = "Чувство",
                Placeholder = "Жажда мести"
            }
        ];

        Info = "«Крутилка» Живорада Славинского - одна из самых тривиальных, но в тоже время действенных техник, которые только могут существовать. Она выполняется за считанные секунды, но результат ощущается уже спустя пару минут. Этот инструмент эффективен для снятия любого негативного заряда из прошлых эпизодов. Всё, что вам нужно, это определить болезненный эпизод и оценить то чувство, которое с ним связано по 10-балльной шкале. Всё гениальное просто!";
    }

}
