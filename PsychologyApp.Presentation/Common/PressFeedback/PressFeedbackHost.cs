using PsychologyApp.Presentation.Common.Behaviors;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Common;

public static class PressFeedbackHost
{
    private static readonly ConditionalWeakTable<ContentPage, object> AttachedPages = new();

    public static void AttachToPage(ContentPage page, PressFeedbackOptions? options = null)
    {
        if (AttachedPages.TryGetValue(page, out _))
        {
            return;
        }

        if (page.Content is not View root)
        {
            void OnLoaded(object? sender, EventArgs e)
            {
                page.Loaded -= OnLoaded;
                AttachToPage(page, options);
            }

            page.Loaded += OnLoaded;
            return;
        }

        if (root.Behaviors.OfType<PressFeedbackBehavior>().Any(b => b.AttachToAllTapTargets))
        {
            AttachedPages.Add(page, null!);
            return;
        }

        PressFeedbackBehavior behavior = new()
        {
            AttachToAllTapTargets = true,
            HapticOnRelease = options?.HapticOnRelease ?? false
        };

        root.Behaviors.Add(behavior);
        AttachedPages.Add(page, null!);
    }
}
