using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.CompanionAvatar;

/// <summary>
/// The companion's identity: a calm gradient orb with a slow breathing ring and an online dot. While <see cref="IsAlive"/>
/// the ring pulses gently outward; with reduced motion it stays still. Pure XAML shapes and standard animations, so nothing
/// here crosses into native drawing code.
/// </summary>
public partial class CompanionAvatarView : ContentView
{
    private const uint BreatheMs = 2200;
    private const double HaloMaxScale = 1.35;
    private const int PauseMs = 500;

    public static readonly BindableProperty IsAliveProperty = BindableProperty.Create(
        nameof(IsAlive),
        typeof(bool),
        typeof(CompanionAvatarView),
        false,
        propertyChanged: (bindable, _, _) => ((CompanionAvatarView)bindable).RestartBreathing());

    public static readonly BindableProperty ShowStatusProperty = BindableProperty.Create(
        nameof(ShowStatus),
        typeof(bool),
        typeof(CompanionAvatarView),
        true,
        propertyChanged: (bindable, _, newValue) => ((CompanionAvatarView)bindable).StatusBadge.IsVisible = (bool)newValue);

    private CancellationTokenSource? _breathing;

    public CompanionAvatarView()
    {
        InitializeComponent();
        Unloaded += (_, _) => StopBreathing();
        Loaded += (_, _) => RestartBreathing();
    }

    /// <summary>Plays the breathing ring. Turn on where the person actually looks at the avatar (chat header, profile); leave off in lists.</summary>
    public bool IsAlive
    {
        get => (bool)GetValue(IsAliveProperty);
        set => SetValue(IsAliveProperty, value);
    }

    /// <summary>The small "online" dot in the corner.</summary>
    public bool ShowStatus
    {
        get => (bool)GetValue(ShowStatusProperty);
        set => SetValue(ShowStatusProperty, value);
    }

    private void RestartBreathing()
    {
        StopBreathing();
        if (!IsAlive || ReduceMotion.IsEnabled || Handler is null)
        {
            return;
        }

        _breathing = new CancellationTokenSource();
        BreatheAsync(_breathing.Token).FireAndForget();
    }

    private void StopBreathing()
    {
        _breathing?.Cancel();
        _breathing = null;
        Halo.Scale = 1;
        Halo.Opacity = 0;
    }

    /// <summary>A soft ring expands and fades from the orb, like a slow, calm pulse. Purely cosmetic: any failure just stops it quietly.</summary>
    private async Task BreatheAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Halo.Scale = 1;
                Halo.Opacity = 0.45;
                await Task.WhenAll(
                    Halo.ScaleToAsync(HaloMaxScale, BreatheMs, Easing.SinOut),
                    Halo.FadeToAsync(0, BreatheMs, Easing.SinIn));
                await Task.Delay(PauseMs, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Leaving the page.
        }
        catch (Exception)
        {
            Halo.Opacity = 0;
        }
    }
}
