using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MobileHelper.ViewModels.TestViewModels;
using MobileHelperMaui.Views.TestPages;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Views.TestPages;
using System.Collections.ObjectModel;

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
                Description = "Тест поможет вам за 5 минут определить ваш уровень депрессии.",
                Algorithm =
                [
                        "1. В этом опроснике содержатся группы утверждений. Внимательно прочитайте каждую группу утверждений",
                        "2. Затем определите в каждой группе одно утверждение, которое лучше всего соответствует тому, как Вы себя чувствовали НА ЭТОЙ НЕДЕЛЕ И СЕГОДНЯ",
                        "3. Поставьте галочку около выбранного утверждения",
                        "4. Если несколько утверждений из одной группы кажутся Вам одинаково хорошо подходящими, то поставьте галочки около каждого из них",
                ],
                Comment = "Прежде, чем сделать свой выбор, убедитесь, что Вы прочли Все утверждения в каждой группе",
                Action = async () => await Navigation.PushAsync(new QuestionPage(
                        [
                            new Question()
                            {
                                Number = 1,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 2,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 3,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 4,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 5,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 6,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 7,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 8,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 9,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 10,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 11,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 12,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 13,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 14,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 15,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 16,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 17,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 18,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 19,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 20,
                                Answers =
                                [
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
                                ]
                            },

                            new Question()
                            {
                                Number = 21,
                                Answers =
                                [
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
                                ]
                            },

                        ],

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

                            else                             {
                                return ball <= 29 ? "20-29 - выраженная депрессия (средней тяжести)" : "30-63 – тяжелая депрессия";
                            }
                        }, false), false

                )
            },

            new()
            {
                Title = "Краткий тест Люшера",
                Subtitle = "Альтернативная версия",
                Description = "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выберите две цветные карточки.",
                Action = async () => await Navigation.PushAsync(new AlternativeTestPage(), false),
                Algorithm =
                [
                    "1. Выбрать самый приятный цвет",
                    "2. Выбрать самый не приятный цвет"
                ],
                Comment = "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",
            },

            new()
            {
                Title = "Полный тест Люшера",
                Subtitle = "Стандартная версия",
                Description = "Тест поможет вам за пару кликов выявить то, что беспокоит, но остаётся неявным. Просто выбирайте карточки в приятной для вас последовательности..",
                Action = async () => await Navigation.PushAsync(new StandardTestPage(), false),
                Algorithm =
                [
                        "1. Выбрать наиболее приятный цвет",
                        "2. Повторить шаг под номером 1"
                ],
                Comment = "Нельзя выбирать любимые цвета. Тест измеряет эмоциональное состояние на сейчас.",

            },

            TestItem.CreateBuilder()
                .SetTitle("Опросник Хекка и Хесса")
                .SetSubtitle("Тест на невроз")
                .SetDescription("Тест поможет вам за 5 минут определить ваш уровень невроза.")
                .SetAlgorithm(
                [
                        "1. Вам представлен перечень утверждений.",
                        "2. На каждое утверждение ответьте «да», если вы с ним согласны (считаете его верным по отношению к себе) или «нет», если вы с ним не согласны.",
                ])
                .SetActionSimple(Navigation,
                (int ball) =>
                {
                            if (ball <= 24)
                            {
                                return "0-24 - невысокий уровень невротизации";
                            }

                            else
                            {
                                return "25-40 - высокий уровень невротизации";
                            }

                }, ["Да", "Нет"], [1, 0],
                [
                    "Считаете ли вы, что внутренне напряжены?",
                    "Я часто так сильно во что-то погружен, что не могу заснуть.",
                    "Я чувствую себя легкоранимым.",
                    "Мне трудно заговорить с незнакомыми людьми.",
                    "Часто ли без особых причин у вас возникает чувство безучастности и усталости.",
                    "У меня часто возникает чувство, что люди меня критически рассматривают.",
                    "Часто ли вас преследуют бесполезные мысли, которые не выходят из головы, хотя вы стараетесь от них избавиться?",
                    "Я довольно нервный.",
                    "Мне кажется, что меня никто не понимает.",
                    "Я довольно раздражительный.",
                    "Если бы против меня не были настроены, мои дела шли бы более успешно.",
                    "Я слишком близко и надолго принимаю к сердцу неприятности.",
                    "Даже мысль о возможной неудаче меня волнует.",
                    "У меня были очень странные и необычные переживания.",
                    "Бывает ли вам то радостно, то грустно без видимых причин?",
                    "В течение всего дня я мечтаю и фантазирую больше, чем нужно.",
                    "Легко ли изменить ваше настроение?",
                    "Я часто борюсь с собой, чтобы не показать своей застенчивости.",
                    "Я хотел бы быть таким же счастливым, каким кажутся другие люди.",
                    "Иногда я дрожу или испытываю приступы озноба.",
                    "Часто ли меняется ваше настроение в зависимости от серьезной причины или без нее?",
                    "Испытываете ли вы иногда чувство страха даже при отсутствии реальной опасности?",
                    "Критика или выговор меня очень ранят.",
                    "Временами я бываю так беспокоен, что даже не могу усидеть на одном месте.",
                    "Беспокоитесь ли вы иногда слишком сильно из-за незначительных вещей?",
                    "Я часто испытываю недовольство.",
                    "Мне трудно сконцентрироваться при выполнении какого-либо задания или работы.",
                    "Я делаю много такого, в чем приходится раскаиваться.",
                    "Большей частью я счастлив.",
                    "Я недостаточно уверен в себе.",
                    "Я страдаю от чувства неполноценности.",
                    "Иногда я кажусь себе действительно никчемным.",
                    "Часто я чувствую себя просто скверно.",
                    "Я много копаюсь в себе.",
                    "Иногда у меня все болит.",
                    "У меня бывает гнетущее состояние.",
                    "У меня что-то с нервами.",
                    "Мне трудно поддерживать разговор при знакомстве.",
                    "Самая тяжелая борьба для меня – это борьба с самим собой.",
                    "Чувствуете ли вы иногда, что трудности велики и непреодолимы?"
                ], true)
                .SetComment("В случае любых сомнений обращайтесь к квалифицированным специалистам.")
                .Build(),

                TestItem.CreateBuilder()
                .SetTitle("Тест Хаэра")
                .SetSubtitle("Тест на психопатию")
                .SetDescription("Тест поможет вам за 5 минут определить ваш уровень психопатии.")
                .SetAlgorithm(
                [
                        "1. Вам представлен перечень утверждений.",
                        "2. На каждое утверждение дайте один из предложенных ответов.",
                ])
                .SetActionSimple(Navigation,
                (int ball) =>
                {
                    if (ball <= 29)
                    {
                        return "0-28 - невысокий уровень психопатии";
                    }

                    else
                    {
                        return "29-40 - высокий уровень психопатии";
                    }

                }, ["Да", "Нет"], [1, 0],
                [
                    "Я никогда никогда не бываю косноязычен.",
                    "В важных отношениях, я выше большинства людей.",
                    "Я подвержен скуке.",
                    "Я лгу чтобы сгладить что-то.",
                    "Я обманываю людей в некоторых вещах.",
                    "Я редко чувствую себя виноватым.",
                    "Я эмоциональный человек.",
                    "Я редко привязываюсь эмоционально к другим.",
                    "За меня часто платят другие.",
                    "Я нетерпелив.",
                    "Я веду беспорядочную половую жизнь.",
                    "Я был трудным ребенком.",
                    "Мне трудно оставаться приверженным долгосрочным целям.",
                    "Я импульсивный.",
                    "Я часто выполняют работу небрежно.",
                    "Я стараюсь уйти от ответственности.",
                    "Мои романтические отношения, как правило, разваливаются быстро.",
                    "Я совершил несколько преступлений будучи несовершеннолетним.",
                    "Я нарушал порядок испытательного срока.",
                    "Я совершил много/несколько видов преступлений.",
                    "Я ни застенчивый, ни стеснительный; я разговариваю с авторитетом, уверенно.",
                    "Я исключительный.",
                    "Мне нужно рисковать, чтобы чувствовать себя живым.",
                    "Я в основном честный человек.",
                    "Я плохо себя чувствую, когда обманываю людей.",
                    "Если кто - то заслуживает наказания, я не чувствую себя слишком плохо.",
                    "Я думаю, что сильные эмоции для слабых.",
                    "Я думаю, если люди обижаются, это их проблема.",
                    "Я всегда забочусь о себе.",
                    "Я никогда не действую поспешно.",
                    "Я думаю, что секс не следует воспринимать легкомысленно.",
                    "Я часто попадал в неприятности в школе.",
                    "Мне не хватает направления в моей жизни.",
                    "Я никогда не поддаюсь соблазну.",
                    "Я всегда держу своё слово.",
                    "Мои проблемы в основном по вине других.",
                    "Я не люблю обязательств в отношениях.",
                    "Я был хулиганом в школе.",
                    "Я проявлял неуважение к суду.",
                    "Я не гордился бы совершив преступления.",
                ], true)
                .SetComment("В случае любых сомнений обращайтесь к квалифицированным специалистам.")
                .Build(),


                TestItem.CreateBuilder()
                .SetTitle("Опросник Л.Г. Почебут")
                .SetSubtitle("Тест агрессивности")
                .SetDescription("Тест поможет вам за 5 минут определить ваш уровень агрессивности.")
                .SetAlgorithm(
                [
                        "1. Вам представлен перечень утверждений.",
                        "2. На каждое утверждение дайте один из предложенных ответов.",
                ])
                .SetActionSimple(Navigation,
                (int ball) =>
                {
                    if (ball <= 29)
                    {
                        return "0-10 - низкий уровень агрессивности";
                    }

                    else if (ball <= 24)
                    {
                        return "11-24 - средний уровень агрессивности";
                    }

                    else 
                    {
                        return "25-40 - высокий уровень агрессивности";
                    }
                }, ["Да", "Нет"], [1, 0],
                [
                    "Во время спора я часто повышаю голос.",
                    "Если меня кто-то раздражает, я могу сказать ему все, что о нем думаю.",
                    "Если мне необходимо будет прибегнуть к физической силе для защиты своих прав, я, не раздумывая, сделаю это.",
                    "Когда я встречаю неприятного мне человека, я могу позволить себе незаметно ущипнуть или толкнуть его.",
                    "Увлекшись спором с другим человеком, я могу стукнуть кулаком по столу, чтобы привлечь к себе внимание или доказать свою правоту.",
                    "Я постоянно чувствую, что другие не уважают мои права.",
                    "Вспоминая прошлое, порой мне бывает обидно за себя.",
                    "Хотя я и не подаю вида, иногда меня гложет зависть.",
                    "Если я не одобряю поведение своих знакомых, то я прямо говорю им об этом.",
                    "В сильном гневе я употребляю крепкие выражения, сквернословлю.",
                    "Если кто-нибудь поднимет на меня руку, я постараюсь ударить его первым.",
                    "Я бываю настолько взбешен, что швыряю разные предметы.",
                    "У меня часто возникает потребность переставить в квартире мебель или полностью сменить ее.",
                    "В общении с людьми я часто чувствую себя «пороховой бочкой», которая постоянно готова взорваться.",
                    "Порой у меня появляется желание зло пошутить над другим человеком.",
                    "Когда я сердит, то обычно мрачнею.",
                    "В разговоре с человеком я стараюсь его внимательно выслушать, не перебивая.",
                    "В молодости у меня часто «чесались кулаки» и я всегда был готов пустить их в ход.",
                    "Если я знаю, что человек намеренно меня толкнул, то дело может дойти до драки.",
                    "Творческий беспорядок на моем рабочем столе позволяет мне эффективно работать.",
                    "Я помню, что бывал настолько сердитым, что хватал все, что попадало под руку, и ломал.",
                    "Иногда люди раздражают меня только одним своим присутствием.",
                    "Я часто удивляюсь, какие скрытые причины заставляют другого человека делать мне что-нибудь хорошее.",
                    "Если мне нанесут обиду, у меня пропадет желание разговаривать с кем бы то ни было.",
                    "Иногда я намеренно говорю гадости о человеке, которого не люблю.",
                    "Когда я взбешен, я кричу самое злобное ругательство.",
                    "В детстве я избегал драться.",
                    "Я знаю, по какой причине и когда можно кого-нибудь ударить.",
                    "Когда я взбешен, то могу хлопнуть дверью.",
                    "Мне кажется, что окружающие люди меня не любят.",
                    "Я постоянно делюсь с другими своими чувствами и переживаниями.",
                    "Очень часто своими словами и действиями я сам себе приношу вред.",
                    "Когда люди орут на меня, я отвечаю тем же.",
                    "Если кто-нибудь ударит меня первым, я в ответ ударю его.",
                    "Меня раздражает, когда предметы лежат не на своем месте.",
                    "Если мне не удается починить сломавшийся или порвавшийся предмет, то я в гневе ломаю или рву его окончательно.",
                    "Другие люди мне всегда кажутся преуспевающими.",
                    "Когда я думаю об очень неприятном мне человеке, я могу прийти в возбуждение от желания причинить ему зло.",
                    "Иногда мне кажется, что судьба сыграла со мной злую шутку.",
                    "Если кто-нибудь обращается со мной не так, как следует, я очень расстраиваюсь по этому поводу.",
                ], true)
                .SetComment("В случае любых сомнений обращайтесь к квалифицированным специалистам.")
                .Build()
        ];
          
    }

}

#region Builder

public interface ITestBuilder
{
    ITestBuilder SetTitle(string title);
    ITestBuilder SetSubtitle(string subtitle);
    ITestBuilder SetDescription(string description);
    ITestBuilder SetComment(string comment);
    ITestBuilder SetAlgorithm(List<string> algorithms);
    ITestBuilder SetActionSimple(INavigation navigation, Func<int, string> analyser, List<string> baseAnswers, List<int> baseBalls, List<string> baseContexts, bool singleAnswer);
    TestItem Build();
}

public class TestBuilder : ITestBuilder
{
    private TestItem TestItem;

    public TestBuilder()
    {
        TestItem = new TestItem();
    }

    public TestItem Build()
    {
        return TestItem;
    }

    public ITestBuilder SetActionSimple(INavigation navigation, Func<int, string> analyser, List<string> baseAnswers, List<int> baseBalls, List<string> baseContexts, bool singleAnswer)
    {
        if (baseAnswers.Count != baseBalls.Count)
        {
            throw new ArgumentException();
        }

        List<Question> questions = new List<Question>();

        for (int contextIndex = 0; contextIndex < baseContexts.Count; contextIndex++)
        {
            List<Answer> answers = new List<Answer>();

            for (int answerIndex = 0; answerIndex < baseAnswers.Count; answerIndex++)
            {
                Answer answer1 = new Answer
                {
                    Number = contextIndex + 1,
                    Ball = baseBalls[answerIndex],
                    Text = baseAnswers[answerIndex],
                    Selected = false
                };

                answers.Add(answer1);
            }

            Question question = new Question
            {
                Number = contextIndex + 1,
                Answers = answers,
                Context = baseContexts[contextIndex],
            };

            questions.Add(question);
        }

        TestItem.Action = async () => await navigation.PushAsync(new QuestionPage(questions, analyser, singleAnswer), false);
        return this;
    }

    public ITestBuilder SetAlgorithm(List<string> algorithms)
    {
        TestItem.Algorithm = algorithms;
        return this;
    }

    public ITestBuilder SetComment(string comment)
    {
        TestItem.Comment = comment;
        return this;
    }

    public ITestBuilder SetDescription(string description)
    {
        TestItem.Description = description;
        return this;
    }

    public ITestBuilder SetSubtitle(string subtitle)
    {
        TestItem.Subtitle = subtitle;
        return this;
    }

    public ITestBuilder SetTitle(string title)
    {
        TestItem.Title = title;
        return this;
    }
}

#endregion

#region Objects
public class TestItem
{
    public string Title { get; set; } = default!;
    public string Subtitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Comment { get; set; } = default!;
    public List<string> Algorithm { get; set; } = default!;
    public Action Action { get; set; } = default!;

    public static TestBuilder CreateBuilder()
    {
        return new TestBuilder();
    }
}

#endregion
