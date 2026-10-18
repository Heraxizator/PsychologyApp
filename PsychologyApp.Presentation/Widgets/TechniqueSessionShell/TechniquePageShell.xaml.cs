using System.Collections;
using System.Windows.Input;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TechniqueSessionShell;

public partial class TechniquePageShell : ContentView
{
    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty AlgorithmProperty =
        BindableProperty.Create(nameof(Algorithm), typeof(IEnumerable), typeof(TechniquePageShell), propertyChanged: OnAlgorithmChanged);

    private static void OnAlgorithmChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var shell = (TechniquePageShell)bindable;
        if (oldValue is System.Collections.Specialized.INotifyCollectionChanged oldSteps)
        {
            oldSteps.CollectionChanged -= shell.OnAlgorithmItemsChanged;
        }

        if (newValue is System.Collections.Specialized.INotifyCollectionChanged newSteps)
        {
            newSteps.CollectionChanged += shell.OnAlgorithmItemsChanged;
        }

        shell.ApplyFold();
    }

    private void OnAlgorithmItemsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => ApplyFold();

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty TheoryCommandProperty =
        BindableProperty.Create(nameof(TheoryCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty FinishCommandProperty =
        BindableProperty.Create(nameof(FinishCommand), typeof(ICommand), typeof(TechniquePageShell));

    public static readonly BindableProperty FinishEnabledProperty =
        BindableProperty.Create(nameof(FinishEnabled), typeof(bool), typeof(TechniquePageShell), true, propertyChanged: (b, _, _) => ((TechniquePageShell)b).ApplyActionBar());

    /// <summary>Controls of the body that belong under the thumb (the step-by-step "Back / Next"): shown in the pinned bar above "Finish".</summary>
    public static readonly BindableProperty FooterContentProperty =
        BindableProperty.Create(nameof(FooterContent), typeof(View), typeof(TechniquePageShell), propertyChanged: OnFooterContentChanged);

    public View? FooterContent
    {
        get => (View?)GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    private static void OnFooterContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var shell = (TechniquePageShell)bindable;
        if (oldValue is View oldFooter)
        {
            oldFooter.PropertyChanged -= shell.OnFooterPropertyChanged;
            shell.FooterHost.Children.Remove(oldFooter);
        }

        if (newValue is View newFooter)
        {
            newFooter.BindingContext = shell.BindingContext;
            newFooter.PropertyChanged += shell.OnFooterPropertyChanged;
            shell.FooterHost.Children.Add(newFooter);
        }

        shell.ApplyActionBar();
    }

    private void OnFooterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsVisible))
        {
            ApplyActionBar();
        }
    }

    /// <summary>The bar takes room only while it has something to press.</summary>
    private void ApplyActionBar()
    {
        if (ActionBar is null)
        {
            return;
        }

        bool footer = FooterContent?.IsVisible == true;
        FooterHost.IsVisible = footer;
        ActionBar.IsVisible = footer || FinishEnabled;
    }

    public static readonly BindableProperty BodyContentProperty =
        BindableProperty.Create(
            nameof(BodyContent),
            typeof(View),
            typeof(TechniquePageShell),
            propertyChanged: OnBodyContentChanged);

    public static readonly BindableProperty TheoryTextProperty =
        BindableProperty.Create(nameof(TheoryText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty AlgorithmTitleTextProperty =
        BindableProperty.Create(nameof(AlgorithmTitleText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public static readonly BindableProperty FinishTextProperty =
        BindableProperty.Create(nameof(FinishText), typeof(string), typeof(TechniquePageShell), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public IEnumerable? Algorithm
    {
        get => (IEnumerable?)GetValue(AlgorithmProperty);
        set => SetValue(AlgorithmProperty, value);
    }

    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ICommand? TheoryCommand
    {
        get => (ICommand?)GetValue(TheoryCommandProperty);
        set => SetValue(TheoryCommandProperty, value);
    }

    public ICommand? FinishCommand
    {
        get => (ICommand?)GetValue(FinishCommandProperty);
        set => SetValue(FinishCommandProperty, value);
    }

    public bool FinishEnabled
    {
        get => (bool)GetValue(FinishEnabledProperty);
        set => SetValue(FinishEnabledProperty, value);
    }

    public View? BodyContent
    {
        get => (View?)GetValue(BodyContentProperty);
        set => SetValue(BodyContentProperty, value);
    }

    public string TheoryText
    {
        get => (string)GetValue(TheoryTextProperty);
        set => SetValue(TheoryTextProperty, value);
    }

    public string AlgorithmTitleText
    {
        get => (string)GetValue(AlgorithmTitleTextProperty);
        set => SetValue(AlgorithmTitleTextProperty, value);
    }

    public string FinishText
    {
        get => (string)GetValue(FinishTextProperty);
        set => SetValue(FinishTextProperty, value);
    }

    public static readonly BindableProperty HeroIconProperty =
        BindableProperty.Create(nameof(HeroIcon), typeof(string), typeof(TechniquePageShell), string.Empty, propertyChanged: (b, _, _) => ((TechniquePageShell)b).ApplyHero());

    public static readonly BindableProperty HeroSubtitleProperty =
        BindableProperty.Create(nameof(HeroSubtitle), typeof(string), typeof(TechniquePageShell), string.Empty, propertyChanged: (b, _, _) => ((TechniquePageShell)b).ApplyHero());

    public static readonly BindableProperty HeroMetaProperty =
        BindableProperty.Create(nameof(HeroMeta), typeof(string), typeof(TechniquePageShell), string.Empty);

    /// <summary>A <c>TechniqueFlavor</c> name: the group colours of the banner.</summary>
    public static readonly BindableProperty HeroFlavorProperty =
        BindableProperty.Create(nameof(HeroFlavor), typeof(string), typeof(TechniquePageShell), string.Empty, propertyChanged: (b, _, _) => ((TechniquePageShell)b).ApplyHero());

    public string HeroIcon
    {
        get => (string)GetValue(HeroIconProperty);
        set => SetValue(HeroIconProperty, value);
    }

    public string HeroSubtitle
    {
        get => (string)GetValue(HeroSubtitleProperty);
        set => SetValue(HeroSubtitleProperty, value);
    }

    public string HeroMeta
    {
        get => (string)GetValue(HeroMetaProperty);
        set => SetValue(HeroMetaProperty, value);
    }

    public string HeroFlavor
    {
        get => (string)GetValue(HeroFlavorProperty);
        set => SetValue(HeroFlavorProperty, value);
    }

    public bool HasHero => !string.IsNullOrWhiteSpace(HeroSubtitle);

    public static readonly BindableProperty AlgorithmFoldedProperty =
        BindableProperty.Create(nameof(AlgorithmFolded), typeof(bool), typeof(TechniquePageShell), false, propertyChanged: (b, _, _) => ((TechniquePageShell)b).ApplyFold());

    /// <summary>
    /// The algorithm shows as one line ("Steps of the practice: 5") that opens on a tap. Used where the steps are already walked
    /// one by one below, so the answer field is on the first screen instead of after a long list.
    /// </summary>
    public bool AlgorithmFolded
    {
        get => (bool)GetValue(AlgorithmFoldedProperty);
        set => SetValue(AlgorithmFoldedProperty, value);
    }

    private bool _algorithmOpened;

    private void OnStepsSummaryTapped(object? sender, TappedEventArgs e)
    {
        _algorithmOpened = !_algorithmOpened;
        UiHaptics.Tick();
        ApplyFold();
        if (_algorithmOpened)
        {
            UiAnimations.SafeRevealLiteAsync(Steps, allowHidden: true).FireAndForget();
        }
    }

    private void ApplyFold()
    {
        if (StepsSummary is null || Steps is null)
        {
            return;
        }

        int count = Algorithm?.Cast<object>().Count() ?? 0;
        bool folded = AlgorithmFolded && count > 0;
        StepsSummary.IsVisible = folded;
        Steps.IsVisible = !folded || _algorithmOpened;
        StepsSummaryLabel.Text = AppStrings.TechniqueStepsSummary(count);
        StepsSummaryChevron.IconName = _algorithmOpened ? "ExpandLess" : "ExpandMore";
        SemanticProperties.SetDescription(StepsSummary, StepsSummaryLabel.Text);
    }

    public TechniquePageShell()
    {
        InitializeComponent();
        ApplyActionBar();
        ApplyLocalization();
        UserPreferences.Changed += ApplyLocalization;
        Loaded += (_, _) =>
        {
            ApplyHero();
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged += OnThemeChanged;
            }
        };
        Unloaded += (_, _) =>
        {
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged -= OnThemeChanged;
            }
        };
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => Dispatcher.Dispatch(ApplyHero);

    /// <summary>The banner takes the colours of the practice's group in the current theme; with no group it stays a plain card.</summary>
    private void ApplyHero()
    {
        OnPropertyChanged(nameof(HasHero));
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        TechniqueFlavor flavor = TechniqueFlavors.Parse(HeroFlavor);
        HeroIconView.IconName = HeroIcon;

        if (TechniqueFlavors.Banner(flavor, dark) is { } banner && TechniqueFlavors.Colors(flavor, dark) is { } colours)
        {
            Hero.Background = new LinearGradientBrush(
                [new GradientStop(Color.FromArgb(banner.Start), 0f), new GradientStop(Color.FromArgb(banner.End), 1f)],
                new Point(0, 0),
                new Point(1, 1));
            HeroTile.BackgroundColor = Color.FromArgb(dark ? "#33FFFFFF" : "#99FFFFFF");
            HeroIconView.IconColor = Color.FromArgb(colours.Icon);
            StepsSummaryIcon.IconColor = Color.FromArgb(colours.Icon);
            Steps.BadgeTint = Color.FromArgb(colours.Tile);
            Steps.BadgeInk = Color.FromArgb(colours.Icon);
            return;
        }

        Hero.Background = new SolidColorBrush(Color.FromArgb(dark ? "#1E1E1E" : "#FFFFFF"));
        HeroTile.BackgroundColor = Color.FromArgb(dark ? "#1A2A3D" : "#D6EBFF");
        HeroIconView.ClearValue(PsychologyApp.Presentation.Shared.UI.Components.MaterialIconView.IconColorProperty);
        Steps.ClearValue(PsychologyApp.Presentation.Shared.UI.Components.AlgorithmBoxView.BadgeTintProperty);
        Steps.ClearValue(PsychologyApp.Presentation.Shared.UI.Components.AlgorithmBoxView.BadgeInkProperty);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        SyncBodyBindingContext();
    }

    private static void OnBodyContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TechniquePageShell shell)
        {
            return;
        }

        shell.UpdateBodyHost(oldValue as View, newValue as View);
    }

    private void UpdateBodyHost(View? oldBody, View? newBody)
    {
        if (oldBody is not null)
        {
            BodyHost.Children.Remove(oldBody);
        }

        if (newBody is null)
        {
            return;
        }

        newBody.BindingContext = BindingContext;
        BodyHost.Children.Add(newBody);
    }

    private void SyncBodyBindingContext()
    {
        if (BodyContent is not null)
        {
            BodyContent.BindingContext = BindingContext;
        }

        if (FooterContent is not null)
        {
            FooterContent.BindingContext = BindingContext;
        }
    }

    private void ApplyLocalization()
    {
        TheoryText = AppStrings.TechniqueTheory;
        AlgorithmTitleText = AppStrings.TechniqueAlgorithm;
        FinishText = AppStrings.TechniqueFinish;
    }
}
