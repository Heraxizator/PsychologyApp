using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public class PolarityViewModel : BaseViewModel
{
    public ICommand Add { get; private set; } = default!;
    public Command<Polarity> Delete { get; private set; } = default!;
    public ObservableCollection<Polarity> polarities { get; private set; } = [];

    public string FirstPolarityLabel => AppStrings.FirstPolarityLabel;
    public string SecondPolarityLabel => AppStrings.SecondPolarityLabel;
    public string NegativePlaceholder => AppStrings.PolarityNegativePlaceholder;
    public string PositivePlaceholder => AppStrings.PolarityPositivePlaceholder;

    public PolarityViewModel(INavigationService navigationService)
    {
        ApplyTechnique(TechniqueId.Polarity);
        IsFull = false;
        BindNavigation(navigationService.Navigation, navigationService);
        Add = new Command(ToAdd);
        Delete = new Command<Polarity>(DeleteItem);
    }

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(FirstPolarityLabel));
        OnPropertyChanged(nameof(SecondPolarityLabel));
        OnPropertyChanged(nameof(NegativePlaceholder));
        OnPropertyChanged(nameof(PositivePlaceholder));
    }

    private void DeleteItem(Polarity item)
    {
        if (item is null)
        {
            return;
        }

        polarities.Remove(item);
        if (polarities.Count == 0)
        {
            IsFull = false;
        }
    }

    private void ToAdd(object obj)
    {
        if (string.IsNullOrEmpty(Negative) || string.IsNullOrEmpty(Positive))
        {
            return;
        }

        IsFull = true;
        Polarity item = new()
        {
            Id = AppStrings.PoleNumber(polarities.Count + 1),
            Positive = Positive,
            Negative = Negative
        };
        polarities.Add(item);
        Polarity = item;
        Negative = string.Empty;
        Positive = string.Empty;
    }

    private string positive = string.Empty;
    public string Positive
    {
        get => positive;
        set => SetProperty(ref positive, value);
    }

    private string negative = string.Empty;
    public string Negative
    {
        get => negative;
        set => SetProperty(ref negative, value);
    }

    private bool isFull;
    public bool IsFull
    {
        get => isFull;
        set => SetProperty(ref isFull, value);
    }

    private Polarity polarity = default!;
    public Polarity Polarity
    {
        get => polarity;
        set => SetProperty(ref polarity, value);
    }
}
