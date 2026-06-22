using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Common;

public static class DynamicListReveal
{
    public static void Attach(Layout layout)
    {
        layout.ChildAdded += (_, e) =>
        {
            if (e.Element is VisualElement element)
            {
                UiAnimations.SafeFadeInAsync(element, duration: UiAnimations.FastDuration, allowHidden: true).FireAndForget();
            }
        };
    }
}
