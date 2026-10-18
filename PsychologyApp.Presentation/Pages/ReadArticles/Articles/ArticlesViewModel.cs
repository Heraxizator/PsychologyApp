using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Application.Articles;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Widgets.LibraryCard;

namespace PsychologyApp.Presentation.Pages.ReadArticles.Articles;

/// <summary>A section of the list: a group of a grouped CollectionView is a list of its items with a header.</summary>
public sealed class ArticleSectionItem(string title, IEnumerable<LibraryCardItem> articles) : List<LibraryCardItem>(articles)
{
    public string Title { get; } = title;
    public bool HasTitle => Title.Length > 0;
}

/// <summary>
/// The "Articles" top tab of the Library: search, chips for one section or all, and the articles in sections with read and saved marks.
/// </summary>
public sealed class ArticlesViewModel : BaseViewModel
{
    /// <summary>The same back arrow as on every other pushed page (the page hides the system bar).</summary>
    public ICommand BackCommand { get; }

    private const string AllKey = "all";

    private readonly INavigationService _navigation;
    private readonly IReadingService _reading;
    private readonly IDatabaseReadySignal _databaseReady;
    private string _selectedKey = AllKey;
    private string _query = string.Empty;
    private ReadingState _state = new();
    private IReadOnlyList<ArticleSectionItem> _sections = [];
    private string _emptyText = string.Empty;

    public ArticlesViewModel(INavigationService navigation, IReadingService reading, IDatabaseReadySignal databaseReady)
    {
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(GoBackAsync);
        _navigation = navigation;
        _reading = reading;
        _databaseReady = databaseReady;
        SelectFilterCommand = new Command<string?>(key =>
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

    public string PageTitle => AppStrings.ShellTabArticlesShort;
    public string SearchPlaceholder => AppStrings.ArticlesSearchPlaceholder;
    public ICommand SelectFilterCommand { get; }

    public ObservableCollection<FilterChipTabItem> Filters { get; } = [];

    /// <summary>What is typed in the search field; the list follows it as it is typed.</summary>
    public string Query
    {
        get => _query;
        set
        {
            if (SetProperty(ref _query, value ?? string.Empty))
            {
                Load();
            }
        }
    }

    private bool IsSearching => _query.Trim().Length >= ArticleSearch.MinQueryLength;

    public IReadOnlyList<ArticleSectionItem> Sections
    {
        get => _sections;
        private set => SetProperty(ref _sections, value);
    }

    public string EmptyText
    {
        get => _emptyText;
        private set
        {
            if (SetProperty(ref _emptyText, value))
            {
                OnPropertyChanged(nameof(HasEmptyText));
            }
        }
    }

    public bool HasEmptyText => EmptyText.Length > 0;

    /// <summary>Called whenever the page appears: an article read or saved meanwhile changes its marks.</summary>
    public async Task RefreshAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            ReadingState state = await _reading.GetAsync();
            await UiThread.RunAsync(() =>
            {
                // The page appears again after every article: rebuild the list only when a read or saved mark actually changed.
                bool marksChanged = !state.Read.SequenceEqual(_state.Read) || !state.Saved.SequenceEqual(_state.Saved);
                _state = state;
                if (marksChanged)
                {
                    Load();
                }
            });
        }
        catch (Exception loadError)
        {
            // The articles are in the app; only the read and saved marks are missing.
            BestEffort.Report(loadError);
        }
    }

    private bool English => AppStrings.IsEnglish(AppStrings.Language);

    /// <summary>The section chips: built once, and again only when the language changes (a tap just moves the selection).</summary>
    private void BuildFilters()
    {
        Filters.Clear();
        Filters.Add(new FilterChipTabItem { Key = AllKey, Title = AppStrings.ArticlesFilterAll, IsSelected = _selectedKey == AllKey });
        foreach (ArticleSection section in ArticleContent.Sections(English))
        {
            Filters.Add(new FilterChipTabItem { Key = section.Key, Title = section.Title, IsSelected = _selectedKey == section.Key });
        }
    }

    private void Load()
    {
        if (IsSearching)
        {
            IReadOnlyList<Article> found = ArticleSearch.Find(_query, English);
            Sections = found.Count == 0 ? [] : [new ArticleSectionItem(string.Empty, found.Select(Card))];
            EmptyText = found.Count == 0 ? AppStrings.ArticlesNothingFound : string.Empty;
            return;
        }

        EmptyText = string.Empty;
        Sections = ArticleContent.Sections(English)
            .Where(section => _selectedKey == AllKey || section.Key == _selectedKey)
            .Select(section => new ArticleSectionItem(section.Title, section.Articles.Select(Card)))
            .ToList();
    }

    private LibraryCardItem Card(Article article) => LibraryCards.Article(article, _state, _navigation);

    protected override void RefreshLocalizedProperties()
    {
        BuildFilters();
        Load();
        Notify(nameof(PageTitle), nameof(SearchPlaceholder));
    }
}
