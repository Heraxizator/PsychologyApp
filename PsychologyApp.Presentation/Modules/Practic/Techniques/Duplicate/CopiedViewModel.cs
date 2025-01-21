using PsychologyApp.Presentation.Models;
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
        this.Navigation = navigation;

        this.Title = "Техника";
        this.Info = "Учёные провели эксперимент и выявили одну замечательную закономерность: если взять то желание, которое имеет слишком большой эмоциональный заряд, записать его на бумаге и много раз повторить, то напряжение ослабнет и станет спокойнее. Тривиально, но это работает!";

        this.Finish = new Command(ToFinish);
        this.Theory = new Command(ToTheory);
        this.AddCommand = new Command(ToAdd);
    }

    private void SetCollection(bool visible)
    {
        this.IsFull = visible;
    }

    private void ToAdd(object obj)
    {
        if (string.IsNullOrWhiteSpace(this.Text) is true)
        {
            return;
        }

        SetCollection(true);

        Paper item = new()
        {
            Id = "Карточка №" + (this.PapersObservableCollection.Count + 1),
            Text = this.Text
        };

        this.PapersObservableCollection.Add(item);
    }

    public Command<Paper> DeleteCommand => new((item) =>
    {
        if (item is null)
        {
            return;
        }

        this.PapersObservableCollection.Remove(item);

        SetCollection(this.PapersObservableCollection.Any());
    });


    private string text = default!;
    public string Text
    {
        get => this.text;
        set
        {
            if (this.text != value)
            {
                this.text = value;
                OnPropertyChanged(nameof(this.Text));
            }
        }
    }

    private bool isFull = default!;
    public bool IsFull
    {
        get => this.isFull;
        set
        {
            if (this.isFull != value)
            {
                this.isFull = value;
                OnPropertyChanged(nameof(this.IsFull));
            }
        }
    }
}

