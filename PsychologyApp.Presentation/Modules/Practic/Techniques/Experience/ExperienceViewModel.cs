﻿

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

        Algorithm =
        [
            "1. Выберите опыт для проработки",
            "2. Где вы находитесь в этом опыте?",
            "3. Когда это происходит?",
            "4. Кто важные участники этого события?",
            "5. Кратко опишите этот опыт",
            "6. Оцените этот опыт по шкале от -10 до 10",
            "7. Какая часть этот опыта была самой нежелательной для вас?",
            "8. Что должно происходить иначе в этом опыте, чтобы он был более приемлемым для вас?",
            "9. Оцените новый опыт по шкале от -10 до 10",
            "10. Если новый опыт оценен меньше, чем на 10, перейдите опять на шаг 7. Повторяйте шаги 7-10 до тех пор, пока новый опыт не станет идеальным (с оценкой 10 из 10)"
        ];

        Entries = [];

        Navigation = navigation;
        Info = "Техника модификации опыта (ТМО) — это эффективный инструмент психологической помощи, позволяющий разобраться как с устоявшимися, так и с недавно появившимися ограничениями, убеждениями, моделями поведения в различных ситуациях. ТМО подходит для самокоучинга. Суть ТМО — позитивное перепроживание ситуаций, давших вам негативный опыт.";
    }

}
