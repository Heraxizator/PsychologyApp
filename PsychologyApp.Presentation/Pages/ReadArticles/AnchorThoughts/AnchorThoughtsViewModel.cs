using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.AnchorThoughts;
using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ReadArticles.AnchorThoughts;

/// <summary>One anchor thought as shown: the thought, why it holds, and the link to the article that explains it.</summary>
public sealed record AnchorThoughtItem(string Text, string Why, string ReadMoreText, ICommand OpenArticleCommand);

/// <summary>
/// The "Anchor thoughts" page of the menu: chips for one theme or all, and the thoughts as cards, each with its reason and an article.
/// The help button stays at the end of the list, because someone looking for a thought to hold on to may need more than a thought.
/// </summary>
public sealed class AnchorThoughtsViewModel : BaseViewModel
{
    private const string AllKey = "all";

    private readonly INavigationService _navigation;
    private string _selectedKey = AllKey;
    private IReadOnlyList<AnchorThoughtItem> _items = [];

    public AnchorThoughtsViewModel(INavigationService navigation)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        BackCommand = new AsyncCommand(GoBackAsync);
        OpenCrisisHubCommand = new AsyncCommand(() => navigation.GoToCrisisHubAsync());
        SelectThemeCommand = new Command<string?>(key =>
        {
            _selectedKey = string.IsNullOrEmpty(key) ? AllKey : key;
            UiHaptics.Tick();
            foreach (FilterChipTabItem filter in Filters)
            {
                filter.IsSelected = filter.Key == _selectedKey;
            }

            Load();
        });
        BuildFilters();
        Load();
    }

    /// <summary>The same back arrow as on every other pushed page (the page hides the system bar).</summary>
    public ICommand BackCommand { get; }

    public ICommand OpenCrisisHubCommand { get; }

    public ICommand SelectThemeCommand { get; }

    public string PageTitle => AppStrings.AnchorThoughtsTitle;

    public string IntroText => AppStrings.AnchorThoughtsIntro;

    public string HelpNowText => AppStrings.AnchorThoughtsHelpNow;

    public ObservableCollection<FilterChipTabItem> Filters { get; } = [];

    public IReadOnlyList<AnchorThoughtItem> Items
    {
        get => _items;
        private set => SetProperty(ref _items, value);
    }

    private static string Language => AppStrings.Language;

    /// <summary>The theme chips: built once, and again only when the language changes (a tap just moves the selection).</summary>
    private void BuildFilters()
    {
        Filters.Clear();
        Filters.Add(new FilterChipTabItem { Key = AllKey, Title = AppStrings.AnchorThemeAll, IsSelected = _selectedKey == AllKey });
        foreach (AnchorTheme theme in Enum.GetValues<AnchorTheme>())
        {
            string key = theme.ToString();
            Filters.Add(new FilterChipTabItem { Key = key, Title = ThemeTitle(theme), IsSelected = _selectedKey == key });
        }
    }

    private void Load()
    {
        string language = Language;
        Items =
        [
            .. AnchorThoughtContent.All(language)
                .Where(thought => _selectedKey == AllKey || thought.Theme.ToString() == _selectedKey)
                .Select(thought => new AnchorThoughtItem(
                    thought.Text,
                    thought.Why,
                    ArticleContent.Find(thought.ArticleId, language) is { } article ? AppStrings.AnchorThoughtsReadMore(article.Title) : string.Empty,
                    new AsyncCommand(() => _navigation.GoToArticleAsync(thought.ArticleId))))
        ];
    }

    private static string ThemeTitle(AnchorTheme theme) => theme switch
    {
        AnchorTheme.OthersWords => AppStrings.AnchorThemeOthersWords,
        AnchorTheme.Mistakes => AppStrings.AnchorThemeMistakes,
        AnchorTheme.ThoughtsAndFeelings => AppStrings.AnchorThemeThoughtsAndFeelings,
        AnchorTheme.Relationships => AppStrings.AnchorThemeRelationships,
        _ => AppStrings.AnchorThemeHardTimes
    };

    protected override void RefreshLocalizedProperties()
    {
        BuildFilters();
        Load();
        Notify(nameof(PageTitle), nameof(IntroText), nameof(HelpNowText));
    }
}
