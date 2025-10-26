using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques;

public class PolarityViewModel : BaseViewModel
{
    public ICommand Add { get; private set; } = default!;
    public string positive { get; private set; } = default!;
    public string negative { get; private set; } = default!;
    public bool isFull { get; private set; } = default!;
    public Polarity polarity { get; private set; } = default!;
    public ObservableCollection<Polarity> polarities { get; private set; } = [];
    public PolarityViewModel() { }

    public PolarityViewModel(INavigation navigation)
    {

        ModuleName = "Практик";
        PageName = "Полярности";

        Algorithm =
        [
            "1. Найти то, что важно",
            "2. Найти то, что противоположно",
            "3. Повторить п.1 и п.2 много раз"
        ];

        Entries =
        [
            new EntryItem
            {
                Title = "Первая полярность",
                Placeholder = "Депрессия"
            },

            new EntryItem {
                Title = "Вторая полярность",
                Placeholder = "Эйфория"
            }
        ];

        Info = "Любой внутренний конфликт связан с борьбой двух противоположных мотивов, желаний, убеждений или целей. По сути причиной многих духовных проблем являются дуальности. Поэтому работа с полярностями - ещё один путь к освобождению от того, что беспокоит. Но, как правило, далеко не одна пара дуальностей создаёт внутренний конфликт. Их может быть несколько. По этой причине рекомендуется рассматривать побольше возможных пар, связанных с проблемой.";
        IsFull = false;
        Navigation = navigation;
        Finish = new Command(ToFinish);
        Add = new Command(ToAdd);
        Theory = new Command(ToTheory);

    }

    public Command<Polarity> Delete => new((item) =>
    {
        if (item is null)
        {
            return;
        }

        _ = polarities.Remove(item);

        if (polarities.Count == 0)
        {
            IsFull = false;
        }
    });

    private void ToAdd(object obj)
    {
        if (!string.IsNullOrEmpty(Negative) && !string.IsNullOrEmpty(Positive))
        {
            IsFull = true;
            Polarity item = new() { Id = "Пара №" + (polarities.Count + 1), Positive = Positive, Negative = Negative };
            polarities.Add(item);
            Polarity = item;

            Negative = "";
            Positive = "";
        }

    }

    public string Positive
    {
        get => positive;
        set
        {
            if (positive != value)
            {
                positive = value;
                OnPropertyChanged(nameof(Positive));
            }
        }
    }

    public string Negative
    {
        get => negative;
        set
        {
            if (negative != value)
            {
                negative = value;
                OnPropertyChanged(nameof(Negative));
            }
        }
    }

    public bool IsFull
    {
        get => isFull;
        set
        {
            if (isFull != value)
            {
                isFull = value;
                OnPropertyChanged(nameof(IsFull));
            }
        }
    }

    public Polarity Polarity
    {
        get => polarity;
        set
        {
            if (polarity != value)
            {
                polarity = value;
                OnPropertyChanged(nameof(Polarity));
            }
        }
    }
}
