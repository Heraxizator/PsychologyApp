using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>Small, dependency-free confetti burst for milestone celebrations — a handful of tinted
/// rectangles that pop from the center, spin, arc up and fall away.</summary>
public partial class ConfettiBurstView : ContentView
{
    private const int PieceCount = 18;
    private const uint Duration = 900;
    private const int MaxStaggerMs = 140;

    private static readonly string[] PaletteKeys =
    [
        "Primary",
        "Success",
        "Yellow100Accent",
        "Cyan100Accent",
        "Blue100Accent"
    ];

    private readonly List<BoxView> _pieces = [];
    private bool _isPlaying;

    public ConfettiBurstView()
    {
        InitializeComponent();
        BuildPieces();
    }

    private void BuildPieces()
    {
        for (int i = 0; i < PieceCount; i++)
        {
            bool tall = i % 2 == 0;
            var piece = new BoxView
            {
                WidthRequest = tall ? 5 : 8,
                HeightRequest = tall ? 11 : 6,
                CornerRadius = 1.5,
                Color = ResolvePieceColor(i),
                Opacity = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                AnchorX = 0.5,
                AnchorY = 0.5
            };
            PieceHost.Children.Add(piece);
            _pieces.Add(piece);
        }
    }

    private static Color ResolvePieceColor(int index)
    {
        string key = PaletteKeys[index % PaletteKeys.Length];
        return Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true
            && value is Color color
            ? color
            : Colors.Gray;
    }

    public async Task PlayAsync()
    {
        if (_isPlaying || !UiAnimations.ShouldAnimate(this))
        {
            return;
        }

        _isPlaying = true;
        try
        {
            Random random = Random.Shared;
            await Task.WhenAll(_pieces.Select(piece => AnimatePieceAsync(piece, random)));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConfettiBurstView.PlayAsync skipped: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            _isPlaying = false;
        }
    }

    private static async Task AnimatePieceAsync(BoxView piece, Random random)
    {
        double dx = (random.NextDouble() * 2 - 1) * 100;
        double rise = -(20 + random.NextDouble() * 50);
        double fall = rise + 110 + random.NextDouble() * 90;
        double rotation = (180 + random.NextDouble() * 520) * (random.NextDouble() < 0.5 ? -1 : 1);
        int delayMs = random.Next(0, MaxStaggerMs);

        piece.TranslationX = 0;
        piece.TranslationY = 0;
        piece.Rotation = 0;
        piece.Scale = 0.6;
        piece.Opacity = 1;

        if (delayMs > 0)
        {
            await Task.Delay(delayMs);
        }

        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var translateX = new Animation(v => piece.TranslationX = v, 0, dx, Easing.CubicOut);
        var riseY = new Animation(v => piece.TranslationY = v, 0, rise, Easing.CubicOut);
        var fallY = new Animation(v => piece.TranslationY = v, rise, fall, Easing.CubicIn);
        var spin = new Animation(v => piece.Rotation = v, 0, rotation, Easing.Linear);
        var pop = new Animation(v => piece.Scale = v, 0.6, 1, Easing.CubicOut);
        var fade = new Animation(v => piece.Opacity = v, 1, 0, Easing.CubicIn);

        var parent = new Animation();
        parent.Add(0, 1, translateX);
        parent.Add(0, 0.35, riseY);
        parent.Add(0.35, 1, fallY);
        parent.Add(0, 1, spin);
        parent.Add(0, 0.2, pop);
        parent.Add(0.55, 1, fade);

        parent.Commit(
            piece,
            "Confetti",
            length: Duration,
            easing: Easing.Linear,
            finished: (_, _) => tcs.TrySetResult(true));

        await tcs.Task;
        piece.Opacity = 0;
    }
}
