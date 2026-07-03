using System.Windows.Input;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public static class ProfilePracticeHistoryTapFactory
{
    public static PracticeHistoryItem WithTapCommand(PracticeHistoryItem item, INavigationService navigationService)
    {
        ICommand? tapCommand = CreateTapCommand(item.ItemKey, navigationService);
        if (tapCommand is null)
        {
            return item;
        }

        return new PracticeHistoryItem
        {
            DateText = item.DateText,
            TechniqueName = item.TechniqueName,
            IconName = item.IconName,
            DurationText = item.DurationText,
            HasDuration = item.HasDuration,
            DisplayText = item.DisplayText,
            ItemKey = item.ItemKey,
            CanOpen = true,
            TapCommand = tapCommand
        };
    }

    private static ICommand? CreateTapCommand(string itemKey, INavigationService navigationService)
    {
        if (string.IsNullOrWhiteSpace(itemKey))
        {
            return null;
        }

        if (itemKey.StartsWith("custom_", StringComparison.Ordinal)
            && long.TryParse(itemKey.AsSpan("custom_".Length), out long customId))
        {
            return new AsyncCommand(() => navigationService.GoToCreatedAsync(customId));
        }

        if (Enum.TryParse(itemKey, out TechniqueId techniqueId))
        {
            return new AsyncCommand(() => navigationService.GoToTechniqueAsync(techniqueId));
        }

        return null;
    }
}
