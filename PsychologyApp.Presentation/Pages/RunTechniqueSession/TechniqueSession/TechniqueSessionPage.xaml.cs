using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.BreathingImmersive;
using PsychologyApp.Presentation.Widgets.BreathingPacer;
using PsychologyApp.Presentation.Widgets.TechniqueBodies;
using PsychologyApp.Presentation.Shared.Common;
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

        // Hidden until the zoom from the tapped card is ready, so the full screen does not flash first.
        _zoomPending = !ReduceMotion.IsEnabled && TapOrigin.HasRecentTap;
        if (_zoomPending)
        {
            ZoomHost.Opacity = 0;
        }
    }

    private bool _zoomPending;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        PlayZoomAsync().FireAndForget();
        InitializeSessionAsync().FireAndForget();
    }

    /// <summary>The screen grows out of the card that was tapped to open it, instead of just appearing.</summary>
    private async Task PlayZoomAsync()
    {
        if (!_zoomPending)
        {
            return;
        }

        _zoomPending = false;
        for (int attempt = 0; attempt < 30 && Width <= 0; attempt++)
        {
            await Task.Delay(16);
        }

        if (TapOrigin.TakeStartFor(Width, Height) is not { } start)
        {
            ZoomHost.Opacity = 1;
            return;
        }

        ZoomHost.AnchorX = 0.5;
        ZoomHost.AnchorY = 0.5;
        ZoomHost.Scale = start.Scale;
        ZoomHost.TranslationX = start.TranslationX;
        ZoomHost.TranslationY = start.TranslationY;
        ZoomHost.Opacity = 1;
        await Task.WhenAll(
            ZoomHost.ScaleToAsync(1, UiAnimations.MediumDuration + 60, Easing.CubicOut),
            ZoomHost.TranslateToAsync(0, 0, UiAnimations.MediumDuration + 60, Easing.CubicOut));
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

        // Breathing gets a circle to breathe with above the usual notes form.
        SessionShell.HeroIcon = definition.ListIcon;
        SessionShell.HeroFlavor = TechniqueFlavors.For(_techniqueId).ToString();
        SessionShell.HeroMeta = AppStrings.TechniqueMetaLine(AppStrings.TechniqueDuration(definition.ListDurationMinutes), definition.Theme);
        SessionShell.HeroSubtitle = definition.ListSubtitle;

        SessionShell.BodyContent = _techniqueId == TechniqueId.Breathing
            ? new VerticalStackLayout { Spacing = 8, Children = { new BreathingPacerView { FullscreenCommand = new Command(OpenImmersiveBreathing) }, body } }
            : body;
    }

    private async void OpenImmersiveBreathing() => await Navigation.PushModalAsync(new BreathingImmersivePage());

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is TechniqueSessionViewModel sessionViewModel)
        {
            sessionViewModel.SaveEntryDraftIfNeeded();
        }
    }
}
