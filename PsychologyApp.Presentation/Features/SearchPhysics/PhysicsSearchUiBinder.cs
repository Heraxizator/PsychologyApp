namespace PsychologyApp.Presentation.Features.SearchPhysics;

public sealed class PhysicsSearchUiBindings
{
    public bool IsSearchEmptyPromptVisible;
    public bool IsSearchFilteringVisible;
    public bool IsSearchResultsListVisible;
    public bool IsSearchNoResultsVisible;
}

public static class PhysicsSearchUiBinder
{
    public static void Apply(
        PhysicsSearchUiBindings bindings,
        PhysicsSearchUiSnapshot snapshot,
        Action<string> notifyPropertyChanged)
    {
        Update(ref bindings.IsSearchEmptyPromptVisible, snapshot.IsSearchEmptyPromptVisible, nameof(PhysicsSearchUiBindings.IsSearchEmptyPromptVisible), notifyPropertyChanged);
        Update(ref bindings.IsSearchFilteringVisible, snapshot.IsSearchFilteringVisible, nameof(PhysicsSearchUiBindings.IsSearchFilteringVisible), notifyPropertyChanged);
        Update(ref bindings.IsSearchResultsListVisible, snapshot.IsSearchResultsListVisible, nameof(PhysicsSearchUiBindings.IsSearchResultsListVisible), notifyPropertyChanged);
        Update(ref bindings.IsSearchNoResultsVisible, snapshot.IsSearchNoResultsVisible, nameof(PhysicsSearchUiBindings.IsSearchNoResultsVisible), notifyPropertyChanged);
    }

    private static void Update(ref bool field, bool value, string propertyName, Action<string> notifyPropertyChanged)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        notifyPropertyChanged(propertyName);
    }
}
