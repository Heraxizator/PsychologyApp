using PsychologyApp.Presentation.Infrastructure;
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

    public string ThoughtFieldLabel => Entries.Count > 0 ? Entries[0].Title ?? string.Empty : string.Empty;
    public string ThoughtPlaceholder => Entries.Count > 0 ? Entries[0].Placeholder ?? string.Empty : string.Empty;
    public string RepeatButtonText => AppStrings.Repeat;
    public string ConcernFieldLabel => AppStrings.ConcernLabel;

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

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(ThoughtFieldLabel));
        OnPropertyChanged(nameof(ThoughtPlaceholder));
        OnPropertyChanged(nameof(RepeatButtonText));
        OnPropertyChanged(nameof(ConcernFieldLabel));
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
            Id = AppStrings.RecordNumber(PapersObservableCollection.Count + 1),
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
