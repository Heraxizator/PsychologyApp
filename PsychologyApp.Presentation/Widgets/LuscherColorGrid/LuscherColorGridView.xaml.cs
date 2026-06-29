using PsychologyApp.Presentation.Shared.Common;
using System.ComponentModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.LuscherColorGrid;

public partial class LuscherColorGridView : ContentView
{
    private const double DefaultTileSize = 88;
    private const double GridGutter = 48;
    private bool _handlersAttached;
    private INotifyPropertyChanged? _boundContext;

    public LuscherColorGridView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
        BindingContextChanged += OnBindingContextChanged;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (_boundContext is not null)
        {
            _boundContext.PropertyChanged -= OnBindingContextPropertyChanged;
            _boundContext = null;
        }

        if (BindingContext is INotifyPropertyChanged newContext)
        {
            _boundContext = newContext;
            _boundContext.PropertyChanged += OnBindingContextPropertyChanged;
        }
    }

    private void OnBindingContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Pages.LuscherTest.LuscherTestViewModel.IsStart) &&
            BindingContext is Pages.LuscherTest.LuscherTestViewModel vm &&
            vm.IsStart)
        {
            ResetTileVisualState();
        }
    }

    private void ResetTileVisualState()
    {
        foreach (Border tile in GetColorTiles())
        {
            tile.Opacity = 1;
            tile.Scale = 1;
        }
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        AttachAnimatedTapHandlers();
        UpdateTileSizes(Width);
    }

    private void OnSizeChanged(object? sender, EventArgs e) => UpdateTileSizes(Width);

    private void UpdateTileSizes(double availableWidth)
    {
        if (availableWidth <= 0)
        {
            return;
        }

        double tile = Math.Min(DefaultTileSize, Math.Max(64, (availableWidth - GridGutter) / 3));

        foreach (Border tileBorder in GetColorTiles())
        {
            tileBorder.WidthRequest = tile;
            tileBorder.HeightRequest = tile;
        }
    }

    private void AttachAnimatedTapHandlers()
    {
        if (_handlersAttached)
        {
            return;
        }

        _handlersAttached = true;

        foreach (Border tile in GetColorTiles())
        {
            if (tile.GestureRecognizers.FirstOrDefault() is not TapGestureRecognizer tap || tap.Command is null)
            {
                continue;
            }

            ICommand original = tap.Command;
            tap.Command = new Command(async () =>
            {
                await UiAnimations.SafePulseAsync(tile);
                await tile.ScaleTo(0.85, 100, Easing.CubicOut);
                await tile.FadeTo(0, 150, Easing.CubicOut);
                if (original.CanExecute(null))
                {
                    original.Execute(null);
                }
            });

            VisualElementPressFeedback.Attach(tile, new PressFeedbackOptions { HapticOnRelease = true });
        }
    }

    private IEnumerable<Border> GetColorTiles() =>
    [
        RedTile, BrownTile, YellowTile,
        GreenTile, BlueTile, PurpleTile,
        GrayTile, BlackTile
    ];
}
