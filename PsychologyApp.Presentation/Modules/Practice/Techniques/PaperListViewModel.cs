using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.ViewModels.TechniqueViewModels;

public class PaperListViewModel : BaseViewModel
{
    private readonly bool _clearTextAfterAdd;

    public ObservableCollection<Paper> PapersObservableCollection { get; private set; } = [];
    public Command AddCommand { get; private set; } = default!;
    public Command<Paper> DeleteCommand { get; private set; } = default!;

    public PaperListViewModel(
        INavigationService navigationService,
        TechniqueId techniqueId,
        bool clearTextAfterAdd)
    {
        _clearTextAfterAdd = clearTextAfterAdd;
        BindNavigation(navigationService.Navigation, navigationService);
        ApplyTechnique(techniqueId);
        AddCommand = new Command(ToAdd);
        DeleteCommand = new Command<Paper>(DeleteItem);
    }

    private void SetCollection(bool visible) => IsFull = visible;

    private void ToAdd(object obj)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        SetCollection(true);
        PapersObservableCollection.Add(new Paper
        {
            Id = "Запись №" + (PapersObservableCollection.Count + 1),
            Text = Text
        });

        if (_clearTextAfterAdd)
        {
            Text = string.Empty;
        }
    }

    private void DeleteItem(Paper item)
    {
        if (item is null)
        {
            return;
        }

        PapersObservableCollection.Remove(item);
        SetCollection(PapersObservableCollection.Any());
    }

    private string text = string.Empty;
    public string Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }
}
