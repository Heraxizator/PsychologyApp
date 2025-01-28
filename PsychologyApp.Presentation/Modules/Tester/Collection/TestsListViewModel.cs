using MobileHelper.ViewModels.TestViewModels;
using MobileHelperMaui.Views.TestPages;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Views.TestPages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Tester.Collection;

public class TestsListViewModel : BaseViewModel
{
    public ObservableCollection<TestItem> TestItemCollection { get; private set; } = [];

    public TestsListViewModel(INavigation navigation)
    {
        Navigation = navigation;

        Init();
    }

    public void Init()
    {
        TestItemCollection =
        [
            new()
            {
                Title = "Опросник Бека",
                Subtitle = "Тест на депрессию",
                ItemTappedCommand = new Command(() => Navigation!.PushAsync(new FindProblemPage(
                    "Тест поможет вам за 5 минут определить ваш уровень депрессии.",
                    [
                        "1. В этом опроснике содержатся группы утверждений. Внимательно прочитайте каждую группу утверждений",
                        "2. Затем определите в каждой группе одно утверждение, которое лучше всего соответствует тому, как Вы себя чувствовали НА ЭТОЙ НЕДЕЛЕ И СЕГОДНЯ",
                        "3. Поставьте галочку около выбранного утверждения",
                        "4. Если несколько утверждений из одной группы кажутся Вам одинаково хорошо подходящими, то поставьте галочки около каждого из них",
                    ],
                    "Прежде, чем сделать свой выбор, убедитесь, что Вы прочли Все утверждения в каждой группе",
                    () => Navigation.PushAsync(new QuestionPage(
                        new List<Question>() 
                        {
                            new Question() 
                            {
                                Number = 1,
                                Answers = new List<Answer>() 
                                {
                                    new Answer 
                                    {
                                        Ball = 0,
                                        Text = "Я не чувствую себя расстроенным, печальным.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я расстроен.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я все время расстроен и не могу от этого отключиться.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я настолько расстроен и несчастлив, что не могу это выдержать.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 2,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не тревожусь о своем будущем.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я чувствую, что озадачен будушим.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я чувствую, что меня ничего не ждет в будущем.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Мое будущее безнадежно, и ничто не может измениться к лучшему.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 3,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не чувствую себя неудачником.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я чувствую, что терпел больше неудач, чем другие люди.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Когда я оглядываюсь на свою жизнь, я вижу в ней много неудач.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я чувствую, что как личность я - полный неудачник.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 4,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я получаю столько же удовлетворения от жизни, как раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я не получаю столько же удовлетворения от жизни, как раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я больше не получаю удовлетворения ни от чего.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я полностью не удовлетворен жизнью. и мне все надоело.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 5,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не чувствую себя в чем-нибудь виноватым.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Достаточно часто я чувствую себя виноватым.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Большую часть времени я чувствую себя виноватым.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я постоянно испытываю чувство вины.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 6,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не чувствую, что могу быть наказанным за что-либо.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я чувствую, что могу быть наказан.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я ожидаю, что могу быть наказан.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я чувствую себя уже наказанным.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 7,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не разочаровался в себе.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я разочаровался в себе.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я себе противен.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я себя ненавижу.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 8,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я знаю, что я не хуже других.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я критикую себя за ошибки и слабости.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я все время обвиняю себя за свои поступки.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я виню себя во всем плохом, что происходит.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 9,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я никогда не думал покончить с собой.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Ко мне приходят мысли покончить с собой, но я не буду их осуществлять.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я хотел бы покончить с собой.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я бы убил себя, если бы представился случай.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 10,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я плачу не больше, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Сейчас я плачу чаще, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Теперь я все время плачу.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Раньше я мог плакать, а сейчас не могу, даже если мне хочется.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 11,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Сейчас я раздражителен не более, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я более легко раздражаюсь, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Теперь я постоянно чувствую, что раздражен.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я стал равнодушен к вещам, которые меня раньше раздражали.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 12,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не утратил интереса к другим людям.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я меньше интересуюсь другими людьми, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я почти потерял интерес к другим людям.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я полностью утратил интерес к другим людям.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 13,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я откладываю принятие решения иногда, как и раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Я чаще, чем раньше, откладываю принятие решения.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Мне труднее принимать решения, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я больше не могу принимать решения.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 14,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я не чувствую, что выгляжу хуже, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Меня тревожит, что я выгляжу старым и непривлекательным.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я знаю, что в моей внешности произошли существенные изменения, делающие меня непривлекательным.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я знаю, что выгляжу безобразно.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 15,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я могу работать так же хорошо, как и раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Мне необходимо сделать дополнительное усилие, чтобы начать делать что-нибудь.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я с трудом заставляю себя делать что-либо.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я совсем не могу выполнять никакую работу.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 16,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я сплю так же хорошо, как и раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Сейчас я сплю хуже, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я просыпаюсь на 1-2 часа раньше, и мне трудно заснуть опять.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я просыпаюсь на несколько часов раньше обычного и больше не могу заснуть.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 17,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я устаю не больше, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Теперь я устаю быстрее, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я устаю почти от всего, что я делаю.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я не могу ничего делать из-за усталости.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 18,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Мой аппетит не хуже, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Мой аппетит стал хуже, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Мой аппетит теперь значительно хуже.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "У меня вообще нет аппетита.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 19,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "В последнее время я не похудел или потеря веса была незначительной.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "За последнее время я потерял более 2 кг.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я потерял более 5 кг.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я потерял более 7 кr.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 20,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "Я беспокоюсь о своем здоровье не больше, чем обычно.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Меня тревожат проблемы моего физического здоровья, такие, как боли, расстройство желудка, запоры и т.д.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Я очень обеспокоен своим физическим состоянием, и мне трудно думать о чем-либо другом.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я настолько обеспокоен своим физическим состоянием, что больше ни о чем не могу думать.",
                                        Selected = false
                                    }
                                }
                            },

                            new Question()
                            {
                                Number = 21,
                                Answers = new List<Answer>()
                                {
                                    new Answer
                                    {
                                        Ball = 0,
                                        Text = "В последнее время я не замечал изменения своего интереса к сексу.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 1,
                                        Text = "Меня меньше занимают проблемы секса, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 2,
                                        Text = "Сейчас я значительно меньше интересуюсь сексуальными проблемами, чем раньше.",
                                        Selected = false
                                    },

                                    new Answer
                                    {
                                        Ball = 3,
                                        Text = "Я полностью утратил сексуальный интерес.",
                                        Selected = false
                                    }
                                }
                            },

                        },

                        (int ball) => 
                        {
                            if (ball <= 9)
                            {
                                return "0-9 - нет депрессивных симптомов";
                            }

                            else if (ball <= 15)
                            {
                                return "10-15 - лёгкая депрессия";
                            }

                            else if (ball <= 19)
                            {
                                return "16-19 - умеренная депрессия";
                            }

                            else if (ball <= 29)
                            {
                                return "20-29 - выраженная депрессия (средней тяжести)";
                            }

                            else
                            {
                                return "30-63 – тяжелая депрессия";
                            }
                        }), false)

                ), false))
            },

            new()
            {
                Title = "Краткий тест Люшера",
                Subtitle = "Альтернативная версия",
                ItemTappedCommand = new Command(() => Navigation!.PushAsync(new FindProblemPage(
                    "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выберите две цветные карточки.",
                    [
                        "1. Выбрать самый приятный цвет",
                        "2. Выбрать самый не приятный цвет"
                    ],
                    "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",
                    () => Navigation.PushAsync(new AlternativeTestPage(), false)

                ), false))
            },

            new()
            {
                Title = "Полный тест Люшера",
                Subtitle = "Стандартная версия",
                ItemTappedCommand = new Command(() => Navigation!.PushAsync(new FindProblemPage(
                    "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выбирайте карточки в приятной для вас последовательности..",
                    [
                        "1. Выбрать наиболее приятный цвет",
                        "2. Повторить шаг под номером 1"
                    ],
                    "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",
                    () => Navigation.PushAsync(new StandardTestPage(), false)

                ), false))
            }
        ];
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
