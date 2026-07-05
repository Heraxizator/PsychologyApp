using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    private bool _hasMoreCustomTechniques;
    private int _customTechniquesOffset;
    private TechniqueGroup? _customTechniquesGroup;
    private bool _isLoadingMoreCustomTechniques;

    public async Task LoadMoreCustomTechniquesAsync()
    {
        if (!_hasMoreCustomTechniques
            || _isLoadingMoreCustomTechniques
            || _customTechniquesGroup is null)
        {
            return;
        }

        _isLoadingMoreCustomTechniques = true;
        try
        {
            int pageSize = CatalogListPolicy.CustomTechniquesPageSize;
            List<TechniqueDTO> page = (await _techniqueService.GetTechniquesPageAsync(
                _customTechniquesOffset,
                pageSize + 1)).ToList();
            bool hasMore = page.Count > pageSize;
            List<TechniqueItem> items = _techniqueListBuilder.MapCustomItems(
                    page.Take(pageSize),
                    _navigationService)
                .ToList();

            await UiThread.RunAsync(() =>
            {
                foreach (TechniqueItem item in items)
                {
                    _customTechniquesGroup.Add(item);
                }

                _customTechniquesOffset += items.Count;
                _hasMoreCustomTechniques = hasMore;
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load more custom techniques.");
        }
        finally
        {
            _isLoadingMoreCustomTechniques = false;
        }
    }

    private void ApplyCustomTechniquesPagingState(TechniquesInitSnapshot snapshot)
    {
        _hasMoreCustomTechniques = snapshot.HasMoreCustomTechniques;
        _customTechniquesOffset = snapshot.CustomTechniquesLoadedCount;
        _customTechniquesGroup = snapshot.UiState.IsGrouped && snapshot.UiState.Groups.Count > 1
            ? snapshot.UiState.Groups[^1]
            : null;
    }
}
