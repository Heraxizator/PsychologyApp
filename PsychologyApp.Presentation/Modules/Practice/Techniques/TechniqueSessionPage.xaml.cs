using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Views.TechniquePages;

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
