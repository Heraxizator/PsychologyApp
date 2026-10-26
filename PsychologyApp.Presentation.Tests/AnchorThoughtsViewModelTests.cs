using Moq;
using PsychologyApp.Application.AnchorThoughts;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Pages.ReadArticles.AnchorThoughts;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class AnchorThoughtsViewModelTests
{
    [Fact]
    public void The_page_opens_with_every_thought_and_a_chip_for_each_theme()
    {
        AnchorThoughtsViewModel viewModel = new(Mock.Of<INavigationService>());

        Assert.Equal(AnchorThoughtContent.All(AppStrings.Language).Select(thought => thought.Text), viewModel.Items.Select(item => item.Text));
        Assert.Equal(Enum.GetValues<AnchorTheme>().Length + 1, viewModel.Filters.Count);
        Assert.True(viewModel.Filters[0].IsSelected);
    }

    // A chip that shows thoughts of another theme, or none, would leave the person without what they came for.
    [Fact]
    public void A_theme_chip_shows_only_the_thoughts_of_that_theme()
    {
        AnchorThoughtsViewModel viewModel = new(Mock.Of<INavigationService>());

        viewModel.SelectThemeCommand.Execute(nameof(AnchorTheme.HardTimes));

        string[] expected = [.. AnchorThoughtContent.All(AppStrings.Language).Where(thought => thought.Theme == AnchorTheme.HardTimes).Select(thought => thought.Text)];
        Assert.NotEmpty(expected);
        Assert.Equal(expected, viewModel.Items.Select(item => item.Text));
        Assert.Single(viewModel.Filters, filter => filter.IsSelected);
        Assert.Equal(nameof(AnchorTheme.HardTimes), viewModel.Filters.Single(filter => filter.IsSelected).Key);
    }

    [Fact]
    public void A_thought_opens_its_article_and_the_help_button_opens_the_crisis_screen()
    {
        Mock<INavigationService> navigation = new();
        AnchorThoughtsViewModel viewModel = new(navigation.Object);
        AnchorThought first = AnchorThoughtContent.All(AppStrings.Language)[0];

        Assert.All(viewModel.Items, item => Assert.False(string.IsNullOrWhiteSpace(item.ReadMoreText)));
        viewModel.Items[0].OpenArticleCommand.Execute(null);
        viewModel.OpenCrisisHubCommand.Execute(null);

        navigation.Verify(n => n.GoToArticleAsync(first.ArticleId), Times.Once);
        navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Once);
    }
}
