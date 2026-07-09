using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    private bool _hasMoreCustomTechniques;
    private int _customTechniquesOffset;
    private TechniqueGroup? _customTechniquesGroup;
    private bool _isLoadingMoreCustomTechniques;
    private CancellationTokenSource? _loadMoreCts;

    public bool IsLoadingMoreCustomTechniques
    {
        get => _isLoadingMoreCustomTechniques;
        private set
        {
            if (SetProperty(ref _isLoadingMoreCustomTechniques, value))
            {
                OnPropertyChanged(nameof(LoadMoreFooterHeight));
            }
        }
    }

    /// <summary>
    /// Collapses CollectionView.Footer when idle — IsVisible=false alone often leaves reserved space.
    /// </summary>
    public double LoadMoreFooterHeight => IsLoadingMoreCustomTechniques ? 72 : 0;

    public async Task LoadMoreCustomTechniquesAsync()
    {
        if (!_hasMoreCustomTechniques
            || IsLoadingMoreCustomTechniques
            || _customTechniquesGroup is null)
        {
            return;
        }

        _loadMoreCts?.Cancel();
        _loadMoreCts?.Dispose();
        _loadMoreCts = OperationCancellation.CreateSmallTimeoutSource(_settings);
        CancellationToken cancellationToken = _loadMoreCts.Token;

        IsLoadingMoreCustomTechniques = true;
        try
        {
            int pageSize = CatalogListPolicy.CustomTechniquesPageSize;
            List<TechniqueDTO> page = (await _techniqueService.GetTechniquesPageAsync(
                _customTechniquesOffset,
                pageSize + 1,
                cancellationToken)).ToList();
            bool hasMore = page.Count > pageSize;
            List<TechniqueItem> items = _techniqueListBuilder.MapCustomItems(
                    page.Take(pageSize),
                    _navigationService)
                .ToList();

            await UiThread.RunAsync(() =>
            {
                if (_customTechniquesGroup is null)
                {
                    return;
                }

                foreach (TechniqueItem item in items)
                {
                    _customTechniquesGroup.Add(item);
                }

                _customTechniquesOffset += items.Count;
                _hasMoreCustomTechniques = hasMore;
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load more custom techniques.");
            _toastService.ShortToast(AppStrings.PracticeLoadMoreError);
        }
        finally
        {
            IsLoadingMoreCustomTechniques = false;
        }
    }

    private void ApplyCustomTechniquesPagingState(TechniquesInitSnapshot snapshot)
    {
        _hasMoreCustomTechniques = snapshot.HasMoreCustomTechniques;
        _customTechniquesOffset = snapshot.CustomTechniquesLoadedCount;
        _customTechniquesGroup = IsTechniquesGrouped && TechniqueGroups.Count > 1
            ? TechniqueGroups[^1]
            : null;
    }
}
