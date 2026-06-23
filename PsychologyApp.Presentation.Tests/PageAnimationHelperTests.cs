using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PageAnimationHelperTests
{
    [Fact]
    public void ShouldSkipContentReveal_SkipsCollectionView()
    {
        CollectionView collectionView = new();

        Assert.True(PageAnimationHelper.ShouldSkipContentReveal(collectionView));
    }

    [Fact]
    public void ShouldSkipContentReveal_SkipsItemsView()
    {
        CarouselView carouselView = new();

        Assert.True(PageAnimationHelper.ShouldSkipContentReveal(carouselView));
    }

    [Fact]
    public void ShouldSkipContentReveal_DoesNotSkipOtherElements()
    {
        VerticalStackLayout layout = new();

        Assert.False(PageAnimationHelper.ShouldSkipContentReveal(layout));
    }
}
