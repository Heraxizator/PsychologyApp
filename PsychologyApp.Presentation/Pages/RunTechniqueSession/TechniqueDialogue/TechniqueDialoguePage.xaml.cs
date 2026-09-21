using System.Collections.Specialized;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;

public partial class TechniqueDialoguePage : ContentPage
{
    private readonly TechniqueDialogueViewModel _viewModel;

    public TechniqueDialoguePage(
        ITechniqueDialogueViewModelFactory viewModelFactory,
        TechniqueId techniqueId,
        INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(techniqueId, hostNavigation);
        BindingContext = _viewModel;
        _viewModel.Messages.CollectionChanged += OnMessagesChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.StartAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Close();
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int last = _viewModel.Messages.Count - 1;
        if (last >= 0)
        {
            MessageList.ScrollTo(last, position: ScrollToPosition.End, animate: true);
        }
    }
}
