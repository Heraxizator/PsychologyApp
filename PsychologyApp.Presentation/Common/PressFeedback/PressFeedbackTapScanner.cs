namespace PsychologyApp.Presentation.Common;

public static class PressFeedbackTapScanner
{
    public static bool ShouldSkipTapTarget(View view) =>
        view is Entry or Editor or Switch or Picker or Slider or Button;

    public static bool HasCommandTap(View view) =>
        view.GestureRecognizers
            .OfType<TapGestureRecognizer>()
            .Any(recognizer => recognizer.Command is not null);

    public static IEnumerable<View> FindTapTargets(VisualElement root)
    {
        if (root is View rootView && IsTapTarget(rootView))
        {
            yield return rootView;
        }

        foreach (View descendant in root.GetVisualTreeDescendants().OfType<View>())
        {
            if (IsTapTarget(descendant))
            {
                yield return descendant;
            }
        }
    }

    private static bool IsTapTarget(View view) =>
        !ShouldSkipTapTarget(view)
        && HasCommandTap(view)
        && !VisualElementPressFeedback.IsAttached(view);
}
