using MobileHelperMaui.Views.TestPages;
using PsychologyApp.Presentation.Views.TestPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.TestViewModels
{
    public class TestsListViewModel : BaseViewModel
    {
        public ObservableCollection<TestItem> TestItemCollection { get; set; }

        public TestsListViewModel()
        {
            Init();
        }

        public void Init()
        {
            this.TestItemCollection = new ObservableCollection<TestItem>()
            {
                new()
                {
                    Title = "Краткий тест Люшера",
                    Subtitle = "Альтернативная версия",
                    ItemTappedCommand = new Command(() => this.Navigation.PushAsync(new FindProblemPage(
                        "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выберите две цветные карточки.",
                        new List<string>()
                        {
                            "1. Выбрать самый приятный цвет",
                            "2. Выбрать самый не приятный цвет"
                        },
                        "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",
                        () => this.Navigation.PushAsync(new AlternativeTestPage(), false)

                    ), false))
                },

                new()
                {
                    Title = "Полный тест Люшера",
                    Subtitle = "Стандартная версия",
                    ItemTappedCommand = new Command(() => this.Navigation.PushAsync(new FindProblemPage(
                        "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выбирайте карточки в приятной для вас последовательности..",
                        new List<string>()
                        {
                            "1. Выбрать наиболее приятный цвет",
                            "2. Повторить шаг под номером 1"
                        },
                        "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",
                        () => this.Navigation.PushAsync(new StandardTestPage(), false)

                    ), false))
                }
            };
        }

        #region Objects

        public class TestItem
        {
            public string? Title { get; set; }
            public string? Subtitle { get; set; }
            public ICommand? ItemTappedCommand { get; set; }

            #endregion
        }
    }
}
