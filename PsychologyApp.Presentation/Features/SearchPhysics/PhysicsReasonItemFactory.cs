using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Physics;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.SearchPhysics;

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
