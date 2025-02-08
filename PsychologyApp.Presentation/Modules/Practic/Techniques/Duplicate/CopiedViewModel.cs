using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace MobileHelperMaui.ViewModels.TechniqueViewModels;

public class CopiedViewModel : BaseViewModel
{
    public ObservableCollection<Paper> PapersObservableCollection { get; private set; } = [];
    public Command AddCommand { get; private set; } = default!;

    public CopiedViewModel() { }

    public CopiedViewModel(INavigation navigation, bool doClear = true)
    {
        Navigation = navigation;

        ModuleName = "Практик";
        PageName = "Повтори Это";

        Algorithm =
        [
            "1. Выписать то, что вас беспокоит",
            "2. Дублировать это до тех пор, пока не станет легче"
        ];

        Entries =
        [
            new EntryItem
            {
                Title = "Беспокойство",
                Placeholder = "Я очень хочу уволиться"
            },
        ];

        Info = "Учёные провели эксперимент и выявили одну замечательную закономерность: если взять то желание, которое имеет слишком большой эмоциональный заряд, записать его на бумаге и много раз повторить, то напряжение ослабнет и станет спокойнее. Тривиально, но это работает!";

        AddCommand = new Command(ToAdd);
    }

    private void SetCollection(bool visible)
    {
        IsFull = visible;
    }

    private void ToAdd(object obj)
    {
        if (string.IsNullOrWhiteSpace(Text) is true)
        {
            return;
        }

        SetCollection(true);

        Paper item = new()
        {
            Id = "Карточка №" + (PapersObservableCollection.Count + 1),
            Text = Text
        };

        PapersObservableCollection.Add(item);
    }

    public Command<Paper> DeleteCommand => new((item) =>
    {
        if (item is null)
        {
            return;
        }

        _ = PapersObservableCollection.Remove(item);

        SetCollection(PapersObservableCollection.Any());
    });


    private string text = default!;
    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }

    private bool isFull = default!;
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
}

