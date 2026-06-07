namespace PsychologyApp.Presentation.Infrastructure;

public static class TemplatePressFeedback
{
    private static readonly HashSet<VisualElement> AttachedTargets = [];

    public static void Attach(ContentView contentView)
    {
        contentView.HandlerChanged += OnHandlerChanged;
        contentView.Loaded += OnLoaded;

        if (contentView.Handler is not null)
        {
            TryAttach(contentView);
        }
    }

    private static void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is ContentView view)
        {
            TryAttach(view);
        }
    }

    private static void OnLoaded(object? sender, EventArgs e)
    {
        if (sender is ContentView view)
        {
            TryAttach(view);
        }
    }

    private static void TryAttach(ContentView contentView)
    {
        VisualElement? target = FindPressTarget(contentView);
        if (target is null || !AttachedTargets.Add(target))
        {
            return;
        }

        VisualElementPressFeedback.Attach(target);
    }

    private static VisualElement? FindPressTarget(VisualElement root)
    {
        if (root is Border border)
        {
            return border;
        }

        foreach (VisualElement child in root.GetVisualTreeDescendants().OfType<VisualElement>())
        {
            if (child is Border pressBorder)
            {
                return pressBorder;
            }
        }

        return root;
    }
}
