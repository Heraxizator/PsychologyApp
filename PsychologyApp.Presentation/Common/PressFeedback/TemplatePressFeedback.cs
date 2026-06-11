using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Common;

public static class TemplatePressFeedback
{
    private static readonly HashSet<VisualElement> AttachedTargets = [];
    private static readonly ConditionalWeakTable<ContentView, PressFeedbackOptions> ContentViewOptions = new();

    public static void Attach(ContentView contentView, PressFeedbackOptions? options = null)
    {
        if (options is not null)
        {
            ContentViewOptions.AddOrUpdate(contentView, options);
        }

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

        ContentViewOptions.TryGetValue(contentView, out PressFeedbackOptions? options);
        VisualElementPressFeedback.Attach(target, options);
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
