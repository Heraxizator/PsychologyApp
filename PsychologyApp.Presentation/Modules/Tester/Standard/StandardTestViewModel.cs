using PsychologyApp.Detector.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.ViewModels.TestViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Tester.Standard;

public class StandardTestViewModel : BaseTestViewModel
{
    public ObservableCollection<ResultItem> ResultItems { get; private set; } = [];

    public StandardTestViewModel() { }


    public StandardTestViewModel(INavigation navigation)
    {
        Navigation = navigation;

        ModuleName = "Детектор";
        PageName = "Стандартный тест Люшера";

        Finish = new Command(ToFinish);
        Restart = new Command(ToRestart);
        BlackHandler = new Command(ToBlackHandler);
        RedHandler = new Command(ToRedHandler);
        BlueHandler = new Command(ToBlueHandler);
        PurpleHandler = new Command(ToPurpleHandler);
        YellowHandler = new Command(ToYellowCommand);
        BrownHandler = new Command(ToBrownHandler);
        GreenHandler = new Command(ToGreenHandler);
        GrayHandler = new Command(ToGrayHandler);

        Init();
    }

    private void ToRestart(object obj)
    {
        Init();
    }

    private void Init()
    {
        _colourSelectedItems.Clear();

        ResultItems = [];

        SetColorsVisibility();

        SetStart();
    }

    protected override void SaveResult(ColourValue colourValue, ColourMeaning colourMeaningVoted, ColourMeaning colourMeaningUnvoted)
    {
        _colourSelectedItems.Add((colourValue, colourMeaningVoted));

        if (_colourSelectedItems.Count == 8)
        {
            ResultItems.Add(new()
            {
                PropertyName = "Суммарное отклонение от аутогенной нормы (СО)",
                PropertyValue = $"{GetCO(_colourSelectedItems)} из 32",
                PropertyText = GetCOText(GetCO(_colourSelectedItems))
            });

            ResultItems.Add(new()
            {
                PropertyName = "Вегетативный коэффициент (ВК)",
                PropertyValue = $"{Math.Round(GetBK(_colourSelectedItems), 2)} из 3.2",
                PropertyText = GetBKText(GetBK(_colourSelectedItems))
            }); ;

            SetFinish();
        }
    }

    internal int GetCO(List<(ColourValue, ColourMeaning)> items)
    {
        int coValue = new();

        for (int key = 1; key < items.Count; key++)
        {
            (ColourValue, ColourMeaning) item = items[key - 1];
            _ = item.Item1.Code;

            int answer = _colourWeights[item.Item1.Code];

            int result = _colourWeights.ElementAt(key - 1).Value;

            coValue += Math.Abs(answer - result);
        }

        return coValue;
    }

    internal string GetCOText(int coValue)
    {
        if (coValue < 6)
        {
            return "Отсутствие непродуктивной (не связанной с какой-либо полезной деятельностью) напряженности, высокая нервно-психическая устойчивость.\r\nДействия обследуемого целесообразны, экономичны, имеют высокий коэффициент полезного действия. Общий эмоциональный настрой – оптимистичный. Обследуемый верит в свои силы и в целом готов преодолевать препятствия и трудности. Высок уровень волевого самоконтроля, предопределяющего поступки и способствующего развитию личности.\r\nПри наличии соответствующей мотивации обследуемый способен интенсивно работать длительное время. В экстремальных ситуациях эффективно мобилизуется, сосредотачивается на выполнении задачи.";
        }

        else
        {
            return coValue < 12
                ? "Незначительный уровень непродуктивной напряженности, нервно-психическая устойчивость хорошая.\r\nПреобладает установка на активность и действие. Энергоресурсов достаточно для более или менее регулярных «подвигов» в работе, вспышек активности и напряжения, недоступных большинству других людей. Способен свободно управлять своим вниманием. В условиях мотивированной (интересной) деятельности не испытывает трудностей с оперативным и долговременным запоминанием и последующим воспроизведением. К острым ощущениям, в общем, не стремится. Из стрессовых ситуаций, как правило, выходит с достоинством."
                : coValue < 17
                            ? "Средний уровень непродуктивной напряженности.\r\nОбследуемый справляется со своими обязанностями в пределах сложившихся в обществе требований. В привычной для него обстановке, имея достаточно времени для переключения, переходит от работы к отдыху и обратно, от одного вида деятельности к другому без существенных затруднений. В случае необходимости способен преодолевать усталость волевым усилием, однако после этого работоспособность надолго снижается. Необходимо относительно четко субъективно разделять время работы и время отдыха."
                            : coValue < 23
                                        ? "Повышенный уровень непродуктивной напряженности, сниженная нервно-психическая устойчивость.\r\nПотенциал целесообразной активности снижен, что побуждает насильно заставлять себя делать те или иные необходимые дела. Постоянно действующий волевой самоконтроль, с одной стороны, и сам регулярно истощается. А с другой – не будучи связанным с непосредственным удовлетворением от процесса и результатов деятельности. Дополнительно усиливает психическое переутомление. Интенсивная длительная работа, скорее всего, потребует слишком большого напряжения от нервной системы и психики. При этом производительность работы и качество ее выполнения будут неравноценными в разные периоды времени. Общий эмоциональный тонус: повышенная возбудимость, тревожность, неуверенность. В стрессовой ситуации вероятно нарушение деятельности."
                                        : "Выраженная непродуктивная напряженность, низкая нервно-психическая устойчивость.\r\nВысокая утомляемость. Внимание легко отвлекается посторонними вещами, надолго может «застрять» на эмоциональном переживании. В связи с этим поведение непрогнозируемо и субъективно. Отсутствие устойчивой иерархии мотивов делает деятельность испытуемого реактивной и нецеленаправленной. Коммуникативность снижена, ограничена рамками формального общения. Эмоциональный фон может быстро колебаться между восторженно-возбужденным состоянием и подавленностью, раздражительностью и бессилием. Часто испытывает тревогу, предчувствие неприятностей, бессилие и отсутствие желания что-либо делать. В экстремальных ситуациях очень низкая надежность.";
        }
    }

    internal double GetBK(List<(ColourValue, ColourMeaning)> items)
    {
        List<ColourValue> colourValues = items.Select(x => x.Item1).ToList();

        double redValue = colourValues.FindIndex(x => x.Code == ColourValue.Red.Code);

        double yellowValue = colourValues.FindIndex(x => x.Code == ColourValue.Yellow.Code);

        double blueValue = colourValues.FindIndex(x => x.Code == ColourValue.Blue.Code);

        double greenValue = colourValues.FindIndex(x => x.Code == ColourValue.Green.Code);

        double bkValue = (18 - redValue - yellowValue) / (18 - blueValue - greenValue);

        return bkValue;
    }

    internal string GetBKText(double bkValue)
    {
        return bkValue <= 0.4
            ? "Истощенность, установка на бездействие. Хроническое переутомление. В связи с этим характерно пассивное реагирование на трудности, неготовность к напряжению и адекватным действиям в стрессовых ситуациях. Необходимы разноплановые и объемные восстановительные мероприятия."
            : bkValue <= 0.8
                ? "Установка на оптимизацию расходования сил. Умеренная потребность в восстановлении и отдыхе. Энергетический потенциал невысок, но вполне достаточен для успешной деятельности в привычных спокойных условиях. В экстремальной ситуации вероятно запаздывание с ориентировкой и принятием решений."
                : bkValue <= 1.9
                            ? "Мобилизованность, установка на активное действие. Оптимальная мобилизованность физических и психических ресурсов. В экстремальной ситуации наиболее вероятна высокая скорость ориентировки и принятия решений, целесообразность и успешность действий."
                            : "Избыточное возбуждение, суетливость. Уровень возбуждения избыточно высок. Нередки случаи, когда испытуемый что-либо делает не ради самого дела, а лишь для того, чтобы разрядиться. В сложных ситуациях легко формируются лихорадочные реакции: импульсивность, нетерпеливость, снижение эмоционального самоконтроля, необдуманные поступки. В экстремальных ситуациях наиболее вероятна низкая эффективность действий, панические реакции. Необходимы разноплановые релаксирующие и успокаивающие процедуры.";
    }

    #region Dictionaries

    private readonly Dictionary<string, int> _colourWeights = new()
    {
        { ColourValue.Red.Code, 1 },
        { ColourValue.Yellow.Code, 2 },
        { ColourValue.Green.Code, 3 },
        { ColourValue.Purple.Code, 4 },
        { ColourValue.Blue.Code, 5 },
        { ColourValue.Brown.Code, 6 },
        { ColourValue.Gray.Code, 7 },
        { ColourValue.Black.Code, 8 }
    };

    #endregion

}

public class ResultItem
{
    public string? PropertyName { get; set; }
    public string? PropertyValue { get; set; }
    public string? PropertyText { get; set; }
}
