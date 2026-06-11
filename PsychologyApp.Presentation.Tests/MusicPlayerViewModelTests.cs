using Microsoft.Extensions.Logging.Abstractions;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.ViewModels.Clean;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class MusicPlayerViewModelTests
{
    public MusicPlayerViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public void InitializePlaylist_LoadsFivePrayersImmediately()
    {
        MusicPlayerViewModel viewModel = new(NullLogger<MusicPlayerViewModel>.Instance);

        Assert.True(viewModel.IsDone);
        Assert.Equal(5, viewModel.AllItems.Count);
        Assert.Equal(5, viewModel.FilteredItems.Count);
    }

    [Fact]
    public void SearchText_FiltersPrayerList()
    {
        MusicPlayerViewModel viewModel = new(NullLogger<MusicPlayerViewModel>.Instance);

        viewModel.SearchText = AppStrings.CleanerPsalm50;

        Assert.Single(viewModel.FilteredItems);
        Assert.Contains(viewModel.FilteredItems, item => item.Name == AppStrings.CleanerPsalm50);
    }

    [Fact]
    public async Task PlayCommand_SetsActiveTrackAndPlaybackFlags()
    {
        MusicPlayerViewModel viewModel = new(NullLogger<MusicPlayerViewModel>.Instance);
        Audio track = viewModel.AllItems[0];
        viewModel.PlayAudioHandler = _ => Task.CompletedTask;

        track.ClickCommand?.Execute(null);
        await Task.Delay(50);

        viewModel.SetPlaybackState(true);

        Assert.True(track.IsActive);
        Assert.True(track.IsPlayingThis);
    }
}
