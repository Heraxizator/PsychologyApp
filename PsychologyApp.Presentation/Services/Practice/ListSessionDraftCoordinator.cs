using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Services.Practice;

public interface IListSessionDraft<TDraftItem>
{
    List<TDraftItem> Items { get; set; }
}

public class ListSessionDraftCoordinator<TItem, TDraft, TDraftItem>
    where TDraft : IListSessionDraft<TDraftItem>, new()
{
    private readonly ILogger? _logger;
    private string _techniqueKey = string.Empty;
    private IUserProgressService _userProgressService = default!;

    protected ListSessionDraftCoordinator(ILogger? logger = null)
    {
        _logger = logger;
    }

    public void Attach(string techniqueKey, IUserProgressService userProgressService)
    {
        _techniqueKey = techniqueKey;
        _userProgressService = userProgressService;
    }

    public async Task LoadAsync(
        ICollection<TItem> target,
        Func<TDraftItem, TItem> map,
        Action<bool> setHasItems)
    {
        TDraft? draft = await SessionDraftStore.LoadAsync<TDraft>(
            _userProgressService,
            _techniqueKey,
            _logger);
        if (draft?.Items is null)
        {
            return;
        }

        await UiThread.RunAsync(() =>
        {
            foreach (TDraftItem item in draft.Items)
            {
                target.Add(map(item));
            }

            setHasItems(target.Count > 0);
        });
    }

    public Task SaveAsync(IEnumerable<TItem> source, Func<TItem, TDraftItem> map) =>
        SessionDraftStore.SaveAsync(
            _userProgressService,
            _techniqueKey,
            new TDraft { Items = source.Select(map).ToList() });
}

public sealed class PaperListDraft : IListSessionDraft<PaperListDraftItem>
{
    public List<PaperListDraftItem> Items { get; set; } = [];
}

public sealed class PaperListDraftItem
{
    public string? Id { get; set; }
    public string? Text { get; set; }
}

public sealed class PaperListDraftCoordinator(ILogger<PaperListDraftCoordinator> logger)
    : ListSessionDraftCoordinator<Models.Practice.Techniques.Paper, PaperListDraft, PaperListDraftItem>(logger);

public sealed class PolarityListDraft : IListSessionDraft<PolarityListDraftItem>
{
    public List<PolarityListDraftItem> Items { get; set; } = [];
}

public sealed class PolarityListDraftItem
{
    public string? Id { get; set; }
    public string? Positive { get; set; }
    public string? Negative { get; set; }
}

public sealed class PolarityListDraftCoordinator(ILogger<PolarityListDraftCoordinator> logger)
    : ListSessionDraftCoordinator<Models.Practice.Techniques.Polarity, PolarityListDraft, PolarityListDraftItem>(logger);
