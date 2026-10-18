using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Widgets.ArticleBlocks;

/// <summary>Turns the blocks of an article or a course day into what the screen shows, links and interactive parts included.</summary>
public static class ArticleBlockItems
{
    public static IReadOnlyList<ArticleBlockItem> Build(IEnumerable<ArticleBlock> blocks, INavigationService navigation) =>
        blocks.Select(block => ToItem(block, navigation)).ToList();

    private static ArticleBlockItem ToItem(ArticleBlock block, INavigationService navigation) => block.Kind switch
    {
        ArticleBlockKind.Bullets => new ArticleBlockItem
        {
            Kind = block.Kind,
            BulletsText = string.Join("\n", (block.Items ?? []).Select(item => $"•  {item}"))
        },
        ArticleBlockKind.Practice when block.Technique is { } technique => Link(block, AppStrings.ArticlePracticeCaption, "SelfImprovement",
            () => navigation.GoToTechniqueAsync(technique)),
        ArticleBlockKind.FearLadder => Link(block, AppStrings.ArticleProgrammeCaption, "Stairs", navigation.GoToExposureAsync),
        ArticleBlockKind.CrisisHelp => Link(block, AppStrings.CrisisHubTitle, "HealthAndSafety", navigation.GoToCrisisHubAsync),
        ArticleBlockKind.Tests => Link(block, AppStrings.ArticleTestsCaption, "Assignment",
            () => block.TestId is { } testId ? navigation.GoToTestAsync(testId) : navigation.GoToTestsTabAsync()),
        ArticleBlockKind.Article when block.ArticleId is { } articleId => Link(block, AppStrings.ArticleReadCaption, "Article",
            () => navigation.GoToArticleAsync(articleId)),
        ArticleBlockKind.TraumaChat => Link(block, AppStrings.ArticleChatCaption, "Forum",
            () => navigation.GoToChatAsync(topic: PsychologyApp.Application.Chat.CompanionTraumaContent.Topic)),
        ArticleBlockKind.Checklist => Checklist(block),
        ArticleBlockKind.Quiz => Quiz(block),
        ArticleBlockKind.Cycle => new ArticleBlockItem
        {
            Kind = block.Kind,
            Text = block.Text,
            Choices = (block.Items ?? []).Select((step, index) => new ArticleChoiceItem { Text = step, Index = index + 1 }).ToList(),
            CycleDescription = $"{block.Text}: {string.Join(" → ", block.Items ?? [])}. {AppStrings.ArticleCycleLoop}"
        },
        _ => new ArticleBlockItem { Kind = block.Kind, Text = block.Text }
    };

    private static ArticleBlockItem Link(ArticleBlock block, string caption, string icon, Func<Task> open) => new()
    {
        Kind = block.Kind,
        Text = block.Text,
        LinkCaption = caption,
        LinkIcon = icon,
        LinkCommand = new AsyncCommand(open)
    };

    private static ArticleBlockItem Checklist(ArticleBlock block)
    {
        List<ArticleChoiceItem> statements = [.. (block.Items ?? []).Select((text, index) => new ArticleChoiceItem { Text = text, Index = index })];
        ArticleBlockItem item = new() { Kind = block.Kind, Text = block.Text, Choices = statements };
        foreach (ArticleChoiceItem statement in statements)
        {
            statement.TapCommand = new Command(() =>
            {
                statement.IsChecked = !statement.IsChecked;
                UiHaptics.Tick();
                int ticked = statements.Count(choice => choice.IsChecked);
                item.Feedback = ticked == 0 ? string.Empty : ticked >= block.Threshold ? block.Verdict ?? string.Empty : block.VerdictLow ?? string.Empty;
            });
        }

        return item;
    }

    private static ArticleBlockItem Quiz(ArticleBlock block)
    {
        List<ArticleChoiceItem> options = [.. (block.Items ?? []).Select((text, index) => new ArticleChoiceItem { Text = text, Index = index, IsQuizOption = true })];
        ArticleBlockItem item = new() { Kind = block.Kind, Text = block.Text, Choices = options };
        foreach (ArticleChoiceItem option in options)
        {
            option.TapCommand = new Command(() =>
            {
                // One try: after an answer the right option is shown either way, with why.
                if (item.HasFeedback)
                {
                    return;
                }

                bool right = option.Index == block.Correct;
                option.IsChecked = true;
                option.IsWrong = !right;
                options[block.Correct].IsRight = true;
                UiHaptics.Tick();
                item.Feedback = $"{(right ? AppStrings.ArticleRight : AppStrings.ArticleNotQuite)} {block.Explanation}";
            });
        }

        return item;
    }
}
