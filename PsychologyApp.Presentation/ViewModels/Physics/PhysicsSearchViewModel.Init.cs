using System.ComponentModel;
using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public partial class PhysicsSearchViewModel
{
    private bool _initialized;

    public bool HasInitialized => _initialized;

    public Task EnsureInitializedAsync() =>
        _initialized ? Task.CompletedTask : InitOnceAsync();

    private async Task InitOnceAsync()
    {
        await InitAsync();
        _initialized = true;
    }

    private void OnSelfPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IsDone) or nameof(IsInit) or nameof(IsFail))
        {
            UpdateSearchUiState();
        }
    }

    private void CancelInit()
    {
        _initCts?.Cancel();
        CancelProgress();
    }

    private void CancelPendingSearch() =>
        _searchCoordinator.CancelPendingSearch(ref _searchDebounceCts);

    private async Task ReloadAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            CancelPendingSearch();
            await UiThread.RunAsync(() => ResultsObservableCollection.Clear());
            await InitCoreAsync();
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Physics search reload failed.");
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task InitAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            await InitCoreAsync();
        }
        catch (OperationCanceledException)
        {
            await UiThread.RunAsync(() =>
            {
                SetDone();
                UpdateSearchUiState();
            });
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Physics search init failed.");
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task InitCoreAsync()
    {
        CancelPendingSearch();
        _initCts?.Cancel();
        _initCts?.Dispose();
        _initCts = _searchSession.CreateInitCancellationSource(_settings);
        CancellationToken cancellationToken = _initCts.Token;

        await UiThread.RunAsync(SetInit);
        await _searchSession.LoadReasonsAsync(_reasonSearchService, _databaseReadySignal, cancellationToken);

        await UiThread.RunAsync(() =>
        {
            SetDone();
            UpdateSearchUiState();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                IsSearching = true;
                RunSearchAsync(SearchText, cancellationToken).FireAndForget();
            }
        });
    }
}
