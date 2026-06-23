using Moq;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Pages.MusicPlayer;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class MusicPlayerViewModelTests
{
    public MusicPlayerViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    private static MusicPlayerViewModel CreateViewModel(Mock<IAudioPlaybackService>? playbackMock = null)
    {
        playbackMock ??= new Mock<IAudioPlaybackService>();
        playbackMock.Setup(p => p.PlayAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        playbackMock.Setup(p => p.Duration).Returns(TimeSpan.FromMinutes(3));
        playbackMock.Setup(p => p.Position).Returns(TimeSpan.Zero);
        return new MusicPlayerViewModel(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<MusicPlayerViewModel>.Instance,
            playbackMock.Object,
            new MusicPlaylistPresenter(),
            new MusicPlaybackPresenter(
                Microsoft.Extensions.Logging.Abstractions.NullLogger<MusicPlaybackPresenter>.Instance,
                new MusicPlaylistPresenter()));
    }

    [Fact]
    public void InitializePlaylist_LoadsVerifiedPrayersImmediately()
    {
        MusicPlayerViewModel viewModel = CreateViewModel();

        Assert.True(viewModel.IsDone);
        Assert.Equal(5, viewModel.AllItems.Count);
        Assert.Equal(5, viewModel.FilteredItems.Count);
        Assert.Equal(4, viewModel.CategoryFilters.Count);
    }

    [Fact]
    public void SearchText_FiltersPrayerList()
    {
        MusicPlayerViewModel viewModel = CreateViewModel();

        viewModel.SearchText = AppStrings.CleanerPsalm50;

        Assert.Single(viewModel.FilteredItems);
        Assert.Equal(AppStrings.CleanerPsalm50, viewModel.FilteredItems[0].Name);
    }

    [Fact]
    public void SelectCategory_FiltersByCategory()
    {
        MusicPlayerViewModel viewModel = CreateViewModel();

        viewModel.SelectCategoryCommand.Execute(AppStrings.CleanerCategoryCore);

        Assert.Equal(3, viewModel.FilteredItems.Count);
        Assert.All(viewModel.FilteredItems, item => Assert.Equal(AppStrings.CleanerCategoryCore, item.Category));
    }

    [Fact]
    public async Task PlayCommand_SetsActiveTrackAndPlaybackFlags()
    {
        Mock<IAudioPlaybackService> playback = new();
        playback.Setup(p => p.PlayAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        MusicPlayerViewModel viewModel = CreateViewModel(playback);
        Audio track = viewModel.AllItems[0];

        track.ClickCommand?.Execute(null);
        await Task.Delay(50);

        Assert.True(track.IsActive);
        playback.Verify(p => p.PlayAsync(track.URL!, It.IsAny<CancellationToken>()), Times.Once);
    }
}
