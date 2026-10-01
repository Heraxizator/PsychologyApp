namespace PsychologyApp.Presentation.Shared.Common;

public static class PressFeedbackTapScanner
{
    public static bool ShouldSkipTapTarget(View view) =>
        view is Entry or Editor or Switch or Picker or Slider or Button;

    // Runs for every element of every page: plain loops, no LINQ allocations.
    public static bool HasCommandTap(View view)
    {
        IList<IGestureRecognizer> recognizers = view.GestureRecognizers;
        for (int i = 0; i < recognizers.Count; i++)
        {
            if (recognizers[i] is TapGestureRecognizer { Command: not null })
            {
                return true;
            }
        }

        return false;
    }

    public static IEnumerable<View> FindTapTargets(VisualElement root)
    {
        List<View> found = [];
        Collect(root, found);
        return found;
    }

    private static void Collect(IVisualTreeElement element, List<View> found)
    {
        if (element is View view && IsTapTarget(view))
        {
            found.Add(view);
        }

        IReadOnlyList<IVisualTreeElement> children = element.GetVisualChildren();
        for (int i = 0; i < children.Count; i++)
        {
            Collect(children[i], found);
        }
    }

    private static bool IsTapTarget(View view) =>
        !ShouldSkipTapTarget(view)
        && HasCommandTap(view)
        && !VisualElementPressFeedback.IsAttached(view);
}
