using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Widgets.TechniqueBodies;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

public partial class TechniqueSessionPage : ContentPage
{
    internal IPageAnalyticsService AnalyticsService { get; }

    public TechniqueSessionPage(
        ITechniqueViewModelFactory techniqueViewModelFactory,
        IPageAnalyticsService pageAnalyticsService,
        TechniqueCatalogGateway techniqueCatalog,
        TechniqueId techniqueId,
        INavigation hostNavigation)
    {
        AnalyticsService = pageAnalyticsService;
        InitializeComponent();
        BaseViewModel viewModel = techniqueViewModelFactory.Create(techniqueId, hostNavigation);
        BindingContext = viewModel;
        View body = TechniqueBodyFactory.Create(techniqueCatalog.Get(techniqueId).UiKind);
        body.BindingContext = viewModel;
        SessionShell.BodyContent = body;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is TechniqueSessionViewModel sessionViewModel)
        {
            sessionViewModel.SaveEntryDraftIfNeeded();
        }
    }
}
