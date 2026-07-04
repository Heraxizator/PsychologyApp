using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Shared.Common;

public static class FavoriteIconAnimator
{
    public static void PulseIfFavoriteChanged(bool oldValue, bool newValue, VisualElement? iconBorder)
    {
        if (oldValue == newValue || iconBorder is null)
        {
            return;
        }

        UiAnimations.SafePulseAsync(iconBorder).FireAndForget();
    }
}
