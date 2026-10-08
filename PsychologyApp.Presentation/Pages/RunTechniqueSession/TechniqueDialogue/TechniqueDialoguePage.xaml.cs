using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDialogue;

public partial class TechniqueDialoguePage : ContentPage
{
    private readonly TechniqueDialogueViewModel _viewModel;
    private readonly KeyboardResize _keyboard = new();

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _keyboard.Enable();
        SafeAsync.Run(_viewModel.StartAsync);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _keyboard.Restore();
    }
}
