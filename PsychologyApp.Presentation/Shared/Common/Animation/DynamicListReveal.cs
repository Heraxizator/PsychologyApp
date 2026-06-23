using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.Common;

public static class DynamicListReveal
{
    public static void Attach(Layout layout)
    {
        layout.ChildAdded += (_, e) =>
        {
            if (e.Element is VisualElement element)
            {
                UiAnimations.SafeRevealLiteAsync(element, allowHidden: true).FireAndForget();
            }
        };
    }
}
