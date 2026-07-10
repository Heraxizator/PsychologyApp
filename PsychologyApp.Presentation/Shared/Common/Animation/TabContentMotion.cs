namespace PsychologyApp.Presentation.Shared.Common;

public static class TabContentMotion
{
    private static readonly string[] RefreshAnchorNames =
    [
        "TechniquesCollectionView",
        "TestsCollectionView",
        "Musics",
        "QuotesCollectionView",
        "ContentStack"
    ];

    public static Task RefreshAsync(ContentPage? page)
    {
        if (page is null || ReduceMotion.IsEnabled)
        {
            return Task.CompletedTask;
        }

        VisualElement? anchor = FindRefreshAnchor(page);
        if (anchor is null)
        {
            return Task.CompletedTask;
        }

        return UiStateAnimator.CrossfadeContentRefreshAsync(anchor);
    }

    private static VisualElement? FindRefreshAnchor(ContentPage page)
    {
        foreach (string name in RefreshAnchorNames)
        {
            if (page.FindByName(name) is VisualElement element)
            {
                return element;
            }
        }

        return null;
    }
}
