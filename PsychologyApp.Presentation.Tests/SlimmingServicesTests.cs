using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Models.Clean;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Profile;
using PsychologyApp.Presentation.Services.Clean;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using System.Collections.ObjectModel;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ProfileQuotesLoaderTests
{
    [Fact]
    public async Task LoadFavoritesAsync_ReturnsMappedItems()
    {
        Mock<IQuotService> quotService = new();
        quotService.Setup(q => q.GetFavouritesAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO { QuotId = 1, Text = "Quote", Title = "Author", IsFavourite = true }
            ]);

        ProfileQuotesLoader loader = new(quotService.Object, new ProfileQuotesPresenter());
        ProfileQuotesLoadResult result = await loader.LoadFavoritesAsync(
            5,
            generation: 1,
            () => 1,
            CancellationToken.None,
            Mock.Of<System.Windows.Input.ICommand>(),
            (_, _) => Mock.Of<System.Windows.Input.ICommand>(),
            (_, _) => Mock.Of<System.Windows.Input.ICommand>());

        Assert.Equal(ProfileQuotesLoadStatus.Ready, result.Status);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CancelLoading_RestoresReadyWhenLoadedOnce()
    {
        Mock<IQuotService> quotService = new();
        quotService.Setup(q => q.GetFavouritesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotDTO { QuotId = 1, Text = "Q", Title = "A", IsFavourite = true }]);

        ProfileQuotesLoader loader = new(quotService.Object, new ProfileQuotesPresenter());
        await loader.LoadFavoritesAsync(
            1,
            1,
            () => 1,
            CancellationToken.None,
            Mock.Of<System.Windows.Input.ICommand>(),
            (_, _) => Mock.Of<System.Windows.Input.ICommand>(),
            (_, _) => Mock.Of<System.Windows.Input.ICommand>());

        ProfileQuotesCancelResult result = loader.CancelLoading(isCurrentlyLoading: true);
        Assert.True(result.ShouldRestoreReady);
    }
}

public sealed class MusicPlaybackPresenterTests
{
    [Fact]
    public void FormatTime_FormatsMinutesAndSeconds()
    {
        Assert.Equal("3:05", MusicPlaybackPresenter.FormatTime(TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public void CreateProgress_CalculatesFraction()
    {
        MusicPlaybackPresenter presenter = new(NullLogger<MusicPlaybackPresenter>.Instance, new MusicPlaylistPresenter());
        MusicPlaybackProgress progress = presenter.CreateProgress(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));

        Assert.Equal("0:30", progress.PositionDisplay);
        Assert.Equal(0.5, progress.ProgressFraction, precision: 2);
    }
}

public sealed class TechniqueDashboardApplierTests
{
    [Fact]
    public void CreateUiState_ReturnsFlatCatalogWhenNoCustomTechniques()
    {
        TechniqueItem item = new() { Title = "Spin" };
        TechniqueListLayout layout = new()
        {
            IsGrouped = false,
            StaticItems = [item],
            CustomItems = [],
            Groups = [],
            ItemsSource = new ObservableCollection<TechniqueItem>([item])
        };

        TechniqueDashboardUiState uiState = TechniqueDashboardApplier.CreateUiState(layout);

        Assert.False(uiState.IsGrouped);
        Assert.Single(uiState.CatalogTechniques);
    }
}

public sealed class QuoteItemCommandsFactoryTests
{
    [Fact]
    public void CreateQuoteItem_AssignsCommands()
    {
        QuoteItemCommandsFactory factory = new(
            new Mock<IQuotService>().Object,
            new QuotesChangeNotifier(),
            Mock.Of<IToastService>(),
            Options.Create(new AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);

        QuoteItem item = factory.CreateQuoteItem(
            new QuotDTO { QuotId = 1, Text = "Text", Title = "Author" },
            _ => Task.CompletedTask,
            () => { });

        Assert.NotNull(item.ShareCommand);
        Assert.NotNull(item.LikeCommand);
        Assert.NotNull(item.CopyCommand);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_RollsBackOnServiceFailure()
    {
        Mock<IQuotService> quotService = new();
        quotService
            .Setup(s => s.MarkAsFavouriteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service failure"));

        QuoteItemCommandsFactory factory = new(
            quotService.Object,
            new QuotesChangeNotifier(),
            Mock.Of<IToastService>(),
            Options.Create(new AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);

        QuoteItem item = new()
        {
            Id = 1,
            Text = "Text",
            Author = "Author",
            IsFavourite = false
        };

        int refreshCount = 0;
        bool failed = false;

        await factory.MarkAsFavouriteAsync(
            item,
            isFavourite: true,
            CancellationToken.None,
            _ =>
            {
                refreshCount++;
                return Task.CompletedTask;
            },
            () => failed = true);

        Assert.False(item.IsFavourite);
        Assert.Equal(2, refreshCount);
        Assert.True(failed);
    }
}
