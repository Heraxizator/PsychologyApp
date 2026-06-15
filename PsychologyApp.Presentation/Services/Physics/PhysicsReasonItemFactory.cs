using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Models.Physics;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Services.Physics;

public static class PhysicsReasonItemFactory
{
    public static PhysicsReasonItem CreateExpandable(
        ReasonDTO dto,
        IReadOnlyList<PhysicsTechniqueSuggestion> suggestions,
        string searchText)
    {
        PhysicsReasonItem item = PhysicsReasonItem.FromDto(dto, suggestions, searchText);
        item.ToggleExpandCommand = new Command(() => item.IsExpanded = !item.IsExpanded);
        return item;
    }
}
