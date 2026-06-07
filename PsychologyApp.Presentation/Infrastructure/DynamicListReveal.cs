using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Infrastructure;

public static class DynamicListReveal
{
    public static void Attach(Layout layout)
    {
        layout.ChildAdded += (_, e) =>
        {
            if (e.Element is VisualElement element)
            {
                UiAnimations.SafeRevealPremiumAsync(element, allowHidden: true).FireAndForget();
            }
        };
    }
}
