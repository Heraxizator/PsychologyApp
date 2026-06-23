using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Widgets.TechniqueBodies;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.TechniqueSession;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.TechniqueSession;

public partial class TechniqueSessionPage : ContentPage
{
    internal IPageAnalyticsService AnalyticsService { get; }

    public TechniqueSessionPage(
        ITechniqueViewModelFactory techniqueViewModelFactory,
        IPageAnalyticsService pageAnalyticsService,
        TechniqueId techniqueId)
    {
        AnalyticsService = pageAnalyticsService;
        InitializeComponent();
        BaseViewModel viewModel = techniqueViewModelFactory.Create(techniqueId, Navigation);
        BindingContext = viewModel;
        View body = TechniqueBodyFactory.Create(TechniqueCatalog.Get(techniqueId).UiKind);
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
