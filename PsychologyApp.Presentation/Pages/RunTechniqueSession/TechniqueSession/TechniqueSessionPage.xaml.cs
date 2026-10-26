using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.BreathingImmersive;
using PsychologyApp.Presentation.Widgets.BalloonPop;
using PsychologyApp.Presentation.Widgets.BreathingPacer;
using PsychologyApp.Presentation.Widgets.BusyMind;
using PsychologyApp.Presentation.Widgets.MuscleRelax;
using PsychologyApp.Presentation.Widgets.TechniqueBodies;
using PsychologyApp.Presentation.Widgets.PracticeStages;
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

        SessionShell.HeroIcon = definition.ListIcon;
        SessionShell.HeroFlavor = TechniqueFlavors.For(_techniqueId).ToString();
        SessionShell.HeroMeta = AppStrings.TechniqueMetaLine(AppStrings.TechniqueDuration(definition.ListDurationMinutes), definition.Theme);
        SessionShell.HeroSubtitle = definition.ListSubtitle;

        // Breathing gets a circle to breathe with above the usual notes form.
        VerticalStackLayout content = new() { Spacing = 8 };
        if (_techniqueId == TechniqueId.Breathing)
        {
            content.Children.Add(new BreathingPacerView { FullscreenCommand = new Command(OpenImmersiveBreathing) });
        }
        else if (_techniqueId == TechniqueId.Balloons)
        {
            content.Children.Add(new BalloonPopView());
        }
        else if (_techniqueId == TechniqueId.CyclicSigh)
        {
            content.Children.Add(new BreathingPacerView { Pattern = BreathingPattern.CyclicSigh });
        }
        else if (_techniqueId == TechniqueId.MuscleRelax)
        {
            content.Children.Add(new MuscleRelaxView());
        }
        else if (_techniqueId == TechniqueId.BusyMind)
        {
            content.Children.Add(new BusyMindView());
        }

        content.Children.Add(body);

        // Practices done in the imagination get a stage where the written problem becomes something to shrink, dissolve or let go.
        if (PracticeStageFactory.Create(_techniqueId) is { } stage)
        {
            stage.BindingContext = BindingContext;
            content.Children.Add(stage);
        }

        // Steps walked one by one, or acted out on a stage, need no long list above them: it folds to one line.
        SessionShell.AlgorithmFolded = (definition.Entries?.Count ?? 0) > 1 || content.Children.Count > 1;
        SessionShell.BodyContent = content;
        if (body is EntryFormBody form)
        {
            SessionShell.FooterContent = form.DetachNavigation();
        }
    }

    // Through the coordinator, so a quick second tap does not stack a second full-screen pacer over the first.
    private void OpenImmersiveBreathing() =>
        SafeAsync.Run(() => NavigationCoordinator.RunPushAsync(() => Navigation.PushModalAsync(new BreathingImmersivePage())));

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is TechniqueSessionViewModel sessionViewModel)
        {
            sessionViewModel.SaveEntryDraftIfNeeded();
        }
    }
}
