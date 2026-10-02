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

        // Unloaded (page popped), not Disappearing: the latter also fires when the app is merely backgrounded.
        Unloaded += (_, _) => _viewModel.Close();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.StartAsync();
        }
        catch (Exception ex)
        {
            // An async void handler must not throw: that would take the whole app down instead of one dialogue.
            PsychologyApp.Presentation.Shared.Common.AsyncCommandExtensions.DefaultErrorHandler?.Invoke(ex);
        }
    }
}
