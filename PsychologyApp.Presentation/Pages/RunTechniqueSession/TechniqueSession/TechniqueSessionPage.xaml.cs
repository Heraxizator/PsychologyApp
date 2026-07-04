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

    private readonly ITechniqueViewModelFactory _techniqueViewModelFactory;
    private readonly TechniqueCatalogGateway _techniqueCatalog;
    private readonly TechniqueId _techniqueId;
    private readonly INavigation _hostNavigation;
    private bool _initialized;
    private bool _bodyLoaded;

    public TechniqueSessionPage(
        ITechniqueViewModelFactory techniqueViewModelFactory,
        IPageAnalyticsService pageAnalyticsService,
        TechniqueCatalogGateway techniqueCatalog,
        TechniqueId techniqueId,
        INavigation hostNavigation)
    {
        AnalyticsService = pageAnalyticsService;
        _techniqueViewModelFactory = techniqueViewModelFactory;
        _techniqueCatalog = techniqueCatalog;
        _techniqueId = techniqueId;
        _hostNavigation = hostNavigation;
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        InitializeSessionAsync().FireAndForget();
    }

    private async Task InitializeSessionAsync()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        BaseViewModel viewModel = await _techniqueViewModelFactory.CreateAsync(_techniqueId, _hostNavigation);
        BindingContext = viewModel;
        await EnsureBodyLoadedAsync();
    }

    private async Task EnsureBodyLoadedAsync()
    {
        if (_bodyLoaded || SessionShell.BodyContent is not null)
        {
            return;
        }

        _bodyLoaded = true;
        TechniqueDefinition definition = await _techniqueCatalog.GetAsync(_techniqueId);
        View body = TechniqueBodyFactory.Create(definition.UiKind);
        body.BindingContext = BindingContext;
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
