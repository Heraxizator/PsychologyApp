using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Entities.Quote;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public enum ProfileQuotesLoadStatus
{
    Ready,
    Failed,
    Cancelled,
    Stale
}

public sealed class ProfileQuotesLoadResult
{
    public ProfileQuotesLoadStatus Status { get; init; }
    public IReadOnlyList<QuoteItem> Items { get; init; } = [];
}

public sealed class ProfileQuotesCancelResult
{
    public bool ShouldRestoreReady { get; init; }
    public bool ShouldSetReadyWithoutData { get; init; }
}

public sealed class ProfileQuotesLoader(
    IQuotService quotService,
    ProfileQuotesPresenter presenter)
{
    private CancellationTokenSource? _loadCts;
    private bool _loadedOnce;

    public bool LoadedOnce => _loadedOnce;

    public void Cancel() => _loadCts?.Cancel();

    public ProfileQuotesCancelResult CancelLoading(bool isCurrentlyLoading)
    {
        Cancel();

        if (!isCurrentlyLoading)
        {
            return new ProfileQuotesCancelResult();
        }

        if (_loadedOnce)
        {
            return new ProfileQuotesCancelResult { ShouldRestoreReady = true };
        }

        return new ProfileQuotesCancelResult { ShouldSetReadyWithoutData = true };
    }

    public async Task<ProfileQuotesLoadResult> LoadFavoritesAsync(
        int count,
        int generation,
        Func<int> getCurrentGeneration,
        CancellationToken outerToken,
        ICommand openQuotesTabCommand,
        Func<string, string, ICommand> shareCommandFactory,
        Func<string, string, ICommand> copyCommandFactory)
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
        _loadCts = CancellationTokenSource.CreateLinkedTokenSource(outerToken);

        try
        {
            IEnumerable<Application.Models.QuotDTO> quotDTOs =
                await quotService.GetFavouritesAsync(count, _loadCts.Token);

            if (generation != getCurrentGeneration())
            {
                return new ProfileQuotesLoadResult { Status = ProfileQuotesLoadStatus.Stale };
            }

            IReadOnlyList<QuoteItem> items = presenter.MapFavorites(
                quotDTOs,
                openQuotesTabCommand,
                shareCommandFactory,
                copyCommandFactory);

            _loadedOnce = true;
            return new ProfileQuotesLoadResult
            {
                Status = ProfileQuotesLoadStatus.Ready,
                Items = items
            };
        }
        catch (OperationCanceledException)
        {
            return new ProfileQuotesLoadResult { Status = ProfileQuotesLoadStatus.Cancelled };
        }
        catch
        {
            if (generation != getCurrentGeneration())
            {
                return new ProfileQuotesLoadResult { Status = ProfileQuotesLoadStatus.Stale };
            }

            return new ProfileQuotesLoadResult { Status = ProfileQuotesLoadStatus.Failed };
        }
    }
}
