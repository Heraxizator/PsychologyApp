using PsychologyApp.Presentation.Models;
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

        this.Title = "Техника";
        Info = "Учёные провели эксперимент и выявили одну замечательную закономерность: если взять лист бумаги, записать свои негативные мысли и выбросить этот лист, то тот негатив потеряют какое-либо значение для человека и перестанет его беспокоить. Но для такой практики совершенно необязательно тратить бумагу. Можно просто воспользоваться текстовым редактором на следующей странице. Техника проста до безобразия!";
        
        this.AddCommand = new Command(ToAdd);
    }

    private void ClearInput()
    {
        this.Text = string.Empty;
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

        ClearInput();
        

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
