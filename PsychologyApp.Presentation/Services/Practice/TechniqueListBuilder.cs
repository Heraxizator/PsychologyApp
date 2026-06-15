using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Services.Practice;

public sealed class TechniqueListLayout
{
    public required bool IsGrouped { get; init; }
    public required IReadOnlyList<TechniqueItem> StaticItems { get; init; }
    public required IReadOnlyList<TechniqueItem> CustomItems { get; init; }
    public required object ItemsSource { get; init; }
    public required IReadOnlyList<TechniqueGroup> Groups { get; init; }
}

public sealed class TechniqueListBuilder(IUserProgressService userProgressService)
{
    public async Task<IReadOnlyList<TechniqueItem>> BuildStaticItemsAsync(
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TechniqueListEntry> entries = TechniqueListCatalog.GetBuiltIn();
        IReadOnlyList<string> keys = entries.Select(entry => entry.TechniqueId.ToString()).ToList();

        Task<IReadOnlyDictionary<string, DateTime>> lastPracticeDatesTask =
            userProgressService.GetLastPracticeDatesAsync(keys, cancellationToken);
        Task<IReadOnlySet<string>> draftKeysTask =
            userProgressService.GetSessionDraftKeysAsync(keys, cancellationToken);

        await Task.WhenAll(lastPracticeDatesTask, draftKeysTask);

        IReadOnlyDictionary<string, DateTime> lastPracticeDates = await lastPracticeDatesTask;
        IReadOnlySet<string> draftKeys = await draftKeysTask;

        List<TechniqueItem> items = [];

        foreach (TechniqueListEntry entry in entries)
        {
            string key = entry.TechniqueId.ToString();
            DateTime? lastPracticeDate = lastPracticeDates.TryGetValue(key, out DateTime lastPractice)
                ? lastPractice
                : null;
            string durationText = AppStrings.TechniqueDuration(entry.DurationMinutes);
            string theme = draftKeys.Contains(key) ? AppStrings.TechniqueContinueBadge : entry.Theme;

            items.Add(new TechniqueItem
            {
                Number = entry.Number,
                Date = lastPracticeDate is null
                    ? AppStrings.TechniqueNotTriedYet
                    : AppStrings.TechniqueLastPractice(lastPracticeDate.Value.ToLocalTime().ToString("d")),
                IconName = entry.Icon,
                DurationText = durationText,
                MetaText = AppStrings.TechniqueMetaLine(durationText, theme),
                Title = entry.Title,
                Subtitle = entry.Subtitle,
                Theme = theme,
                Author = entry.Author,
                Active = true,
                TapCommand = new AsyncCommand(() => navigationService.GoToTechniqueAsync(entry.TechniqueId))
            });
        }

        return items;
    }

    public IReadOnlyList<TechniqueItem> MapCustomItems(
        IEnumerable<TechniqueDTO> techniques,
        INavigationService navigationService) =>
        techniques.Select(item => new TechniqueItem
        {
            Id = item.TechniqueId,
            Number = AppStrings.PracticeCustomTechniqueNumber(item.TechniqueId),
            Date = item.Date,
            Image = item.Image,
            IconName = "Build",
            Title = item.Header,
            Subtitle = item.Description,
            Theme = item.Subject,
            MetaText = item.Subject ?? string.Empty,
            Author = item.Author,
            Active = true,
            TapCommand = new AsyncCommand(() => navigationService.GoToCreatedAsync(item.TechniqueId))
        }).ToList();

    public TechniqueListLayout BuildLayout(
        IReadOnlyList<TechniqueItem> staticItems,
        IReadOnlyList<TechniqueItem> customItems,
        string customGroupLabel)
    {
        if (customItems.Count > 0)
        {
            TechniqueGroup[] groups =
            [
                new TechniqueGroup(string.Empty, staticItems),
                new TechniqueGroup(customGroupLabel, customItems)
            ];
            return new TechniqueListLayout
            {
                IsGrouped = true,
                StaticItems = staticItems,
                CustomItems = customItems,
                Groups = groups,
                ItemsSource = groups
            };
        }

        return new TechniqueListLayout
        {
            IsGrouped = false,
            StaticItems = staticItems,
            CustomItems = customItems,
            Groups = [],
            ItemsSource = new ObservableCollection<TechniqueItem>(staticItems)
        };
    }
}
