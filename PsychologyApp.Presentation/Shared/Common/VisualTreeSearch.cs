namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Depth-first search that stops at the first match. GetVisualTreeDescendants() builds a list of the whole subtree
/// first, which per card or per page load cost far more than finding an element that sits near the root.
/// </summary>
public static class VisualTreeSearch
{
    public static T? FindFirst<T>(IVisualTreeElement root, Func<T, bool>? predicate = null)
        where T : class
    {
        IReadOnlyList<IVisualTreeElement> children = root.GetVisualChildren();
        for (int i = 0; i < children.Count; i++)
        {
            IVisualTreeElement child = children[i];
            if (child is T match && (predicate is null || predicate(match)))
            {
                return match;
            }

            if (FindFirst(child, predicate) is { } nested)
            {
                return nested;
            }
        }

        return null;
    }
}
