using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace MobileHelperMaui.ViewModels.TechniqueViewModels;

public class PaperViewModel : BaseViewModel
{
    public ObservableCollection<Paper> PapersObservableCollection { get; private set; } = [];
    public Command AddCommand { get; private set; } = default!;

    public PaperViewModel() { }

    public PaperViewModel(INavigation navigation, bool doClear = true)
    {
        Navigation = navigation;

        ModuleName = "Практик";
        PageName = "Лист Бумаги";

        Algorithm =
        [
            "1. Выписать негатив на карточку",
            "2. Повторить это много раз",
            "3. Удалить все карточки"
        ];

        Entries =
        [
            new EntryItem
            {
                Title = "Негативная мысль",
                Placeholder = "Мне не хочется жить"
            },
        ];

        Info = "Учёные провели эксперимент и выявили одну замечательную закономерность: если взять лист бумаги, записать свои негативные мысли и выбросить этот лист, то тот негатив потеряют какое-либо значение для человека и перестанет его беспокоить. Но для такой практики совершенно необязательно тратить бумагу. Можно просто воспользоваться текстовым редактором на следующей странице. Техника проста до безобразия!";

        AddCommand = new Command(ToAdd);
    }

    private void ClearInput()
    {
        Text = string.Empty;
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

        ClearInput();


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
