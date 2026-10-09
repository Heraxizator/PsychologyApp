using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageJournal.Journal;

public partial class JournalPage : ContentPage, IJournalHubPage
{
    private const double BloomSize = 120;
    private const uint BloomMs = 1100;

    private readonly JournalViewModel _viewModel;
    private int _lastMoodLevel;

    public JournalPage(IJournalViewModelFactory journalViewModelFactory)
    {
        InitializeComponent();
        _viewModel = journalViewModelFactory.Create(this);
        BindingContext = _viewModel;
        _lastMoodLevel = _viewModel.SelectedMoodLevel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ReloadAsync().FireAndForget();
    }

    protected override void OnDisappearing()
    {
        _viewModel.FlushPendingNoteSaveAsync().FireAndForget();
        base.OnDisappearing();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(JournalViewModel.SelectedMoodLevel))
        {
            return;
        }

        int level = _viewModel.SelectedMoodLevel;
        bool changed = level != _lastMoodLevel;
        _lastMoodLevel = level;

        // Only a face the person has just tapped blooms: a level that arrives from a reload has no tap behind it.
        if (changed && level is >= 1 and <= 5)
        {
            BloomAsync(level).FireAndForget();
        }
    }

    /// <summary>A circle in the colour of the mood opens from the tapped face to the corners of the page and fades, as if the colour were poured into the day.</summary>
    private async Task BloomAsync(int level)
    {
        if (ReduceMotion.IsEnabled || !TapOrigin.TryPeek(out Rect tap) || Width <= 0 || Height <= 0)
        {
            return;
        }

        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        double centreX = tap.X + tap.Width / 2;
        double centreY = tap.Y + tap.Height / 2;
        double reach = Math.Max(Math.Max(centreX, Width - centreX), Math.Max(centreY, Height - centreY));

        MoodBloom.AbortAnimation("bloom");
        MoodBloom.BackgroundColor = Color.FromArgb(MoodPalette.Fill(level, dark));
        MoodBloom.TranslationX = centreX - BloomSize / 2;
        MoodBloom.TranslationY = centreY - BloomSize / 2;
        MoodBloom.Scale = 0.3;
        MoodBloom.Opacity = 0.5;

        double target = reach * 2 / BloomSize;
        await Task.WhenAll(
            MoodBloom.ScaleToAsync(target, BloomMs, Easing.CubicOut),
            MoodBloom.FadeToAsync(0, BloomMs, Easing.CubicIn));
    }
}
