using System.Globalization;
using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

public partial class CompanionProfilePage : ContentPage
{
    private const uint CountUpMs = 900;
    private const uint TrustBarMs = 800;
    private const uint HaloMs = 1800;
    private const uint BarGrowMs = 600;
    private const int BarStaggerMs = 70;
    private const int AfterBarExtraDelayMs = 90;

    private readonly CompanionProfileViewModel _viewModel;
    private CancellationTokenSource? _halo;

    public CompanionProfilePage(IChatViewModelFactory viewModelFactory, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateProfile(hostNavigation);
        _viewModel.PromptAsync = (title, message, accept, cancel, initial) =>
            DisplayPromptAsync(title, message, accept, cancel, initialValue: initial, maxLength: 30);
        BindingContext = _viewModel;
        _viewModel.ProfileLoaded += (_, _) => PlayNumbers();
        Unloaded += (_, _) => StopHalo();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
        StartHalo();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopHalo();
    }

    /// <summary>Numbers count up from zero, the trust bar fills and the level marks light up. With reduced motion everything is set at once.</summary>
    private void PlayNumbers()
    {
        SetOrCount(ChatsValue, _viewModel.Chats, nameof(ChatsValue));
        SetOrCount(MessagesValue, _viewModel.Messages, nameof(MessagesValue));
        SetOrCount(DaysValue, _viewModel.Days, nameof(DaysValue));
        SetOrCount(StreakValue, _viewModel.Streak, nameof(StreakValue));

        double progress = _viewModel.TrustProgress;
        if (ReduceMotion.IsEnabled)
        {
            TrustBar.Progress = progress;
        }
        else
        {
            TrustBar.Progress = 0;
            TrustBar.ProgressTo(progress, TrustBarMs, Easing.CubicOut).FireAndForget();
        }

        Border[] dots = [TrustDot0, TrustDot1, TrustDot2, TrustDot3];
        for (int i = 0; i < dots.Length; i++)
        {
            if (i <= _viewModel.TrustLevel)
            {
                dots[i].BackgroundColor = (Color)Microsoft.Maui.Controls.Application.Current!.Resources["Primary"];
            }
            else
            {
                dots[i].SetAppThemeColor(
                    BackgroundColorProperty,
                    (Color)Microsoft.Maui.Controls.Application.Current!.Resources["Gray100"],
                    (Color)Microsoft.Maui.Controls.Application.Current.Resources["Gray900"]);
            }
        }
    }

    private void SetOrCount(Label label, int target, string name)
    {
        label.AbortAnimation(name);
        if (ReduceMotion.IsEnabled || target == 0)
        {
            label.Text = target.ToString(CultureInfo.InvariantCulture);
            return;
        }

        label.Text = "0";
        new Animation(v => label.Text = ((int)Math.Round(v)).ToString(CultureInfo.InvariantCulture), 0, target)
            .Commit(label, name, 16, CountUpMs, Easing.CubicOut, (_, _) => label.Text = target.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>The bars grow from the baseline one pair after another. If anything goes wrong the bar is simply shown.</summary>
    private void OnBarContextChanged(object? sender, EventArgs e)
    {
        if (sender is not VisualElement bar)
        {
            return;
        }

        bar.ScaleY = 1;
        if (ReduceMotion.IsEnabled || bar.BindingContext is not TensionBarItem item)
        {
            return;
        }

        int index = Math.Max(_viewModel.TensionBars.IndexOf(item), 0);
        int delay = 120 + (index * BarStaggerMs) + (bar.StyleId == "after" ? AfterBarExtraDelayMs : 0);
        bar.ScaleY = 0;
        GrowAsync(bar, delay).FireAndForget();
    }

    private static async Task GrowAsync(VisualElement bar, int delay)
    {
        try
        {
            await Task.Delay(delay);
            await bar.ScaleYTo(1, BarGrowMs, Easing.CubicOut);
        }
        catch (Exception)
        {
            bar.ScaleY = 1;
        }
        finally
        {
            bar.ScaleY = 1;
        }
    }

    private void StartHalo()
    {
        StopHalo();
        if (ReduceMotion.IsEnabled)
        {
            return;
        }

        _halo = new CancellationTokenSource();
        PulseAsync(_halo.Token).FireAndForget();
    }

    private void StopHalo()
    {
        _halo?.Cancel();
        _halo = null;
        Halo.Scale = 1;
        Halo.Opacity = 1;
    }

    /// <summary>A slow ring that breathes out around the face: calm, not attention-grabbing.</summary>
    private async Task PulseAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Halo.Scale = 0.86;
                Halo.Opacity = 0.9;
                await Task.WhenAll(
                    Halo.ScaleToAsync(1.08, HaloMs, Easing.SinOut),
                    Halo.FadeToAsync(0.15, HaloMs, Easing.SinIn));
                await Task.Delay(250, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Leaving the page.
        }
    }
}
