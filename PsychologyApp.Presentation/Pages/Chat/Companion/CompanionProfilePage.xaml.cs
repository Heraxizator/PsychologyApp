using System.Globalization;
using PsychologyApp.Presentation.Features.Chat.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.Companion;

public partial class CompanionProfilePage : ContentPage
{
    private const uint CountUpMs = 900;
    private const uint BarGrowMs = 600;
    private const int BarStaggerMs = 70;
    private const int AfterBarExtraDelayMs = 90;

    private readonly CompanionProfileViewModel _viewModel;

    public CompanionProfilePage(IChatViewModelFactory viewModelFactory, INavigation hostNavigation)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.CreateProfile(hostNavigation);
        _viewModel.PromptAsync = (title, message, accept, cancel, initial) =>
            DisplayPromptAsync(title, message, accept, cancel, initialValue: initial, maxLength: 30);
        BindingContext = _viewModel;
        _viewModel.ProfileLoaded += (_, _) => PlayNumbers();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshAsync().FireAndForget();
    }

    /// <summary>Numbers count up from zero; the trust meter itself is plain data binding now, nothing to drive here. With reduced motion everything is set at once.</summary>
    private void PlayNumbers()
    {
        SetOrCount(ChatsValue, _viewModel.Chats, nameof(ChatsValue));
        SetOrCount(MessagesValue, _viewModel.Messages, nameof(MessagesValue));
        SetOrCount(DaysValue, _viewModel.Days, nameof(DaysValue));
        SetOrCount(StreakValue, _viewModel.Streak, nameof(StreakValue));
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
}
