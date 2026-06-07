using System.ComponentModel;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.Infrastructure;

public sealed class PageAnimationHelper : IDisposable
{
    private readonly BaseViewModel? _viewModel;
    private readonly VisualElement? _loadingView;
    private readonly VisualElement? _contentView;
    private readonly VisualElement? _introView;
    private readonly Layout? _staggerLayout;
    private CancellationTokenSource? _cts;
    private bool _contentRevealed;
    private bool _introDismissed;

    public PageAnimationHelper(
        BaseViewModel? viewModel,
        VisualElement? loadingView = null,
        VisualElement? contentView = null,
        Layout? staggerLayout = null,
        VisualElement? introView = null)
    {
        _viewModel = viewModel;
        _loadingView = loadingView;
        _contentView = contentView;
        _staggerLayout = staggerLayout;
        _introView = introView;

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    public void TryRevealAsync() => ScheduleAnimation(HandleLoadingStateAsync);

    public async Task RevealOnAppearingAsync(VisualElement? root) =>
        await UiAnimations.SafeRevealAsync(root);

    public async Task RevealStaggeredAsync() =>
        await UiAnimations.RevealChildrenStaggeredAsync(_staggerLayout, cancellationToken: _cts?.Token ?? CancellationToken.None);

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(BaseViewModel.IsDone)
            or nameof(BaseViewModel.IsInit)
            or nameof(BaseViewModel.IsCreated))
        {
            ScheduleAnimation(HandleLoadingStateAsync);
        }
    }

    private void ScheduleAnimation(Func<Task> animation)
    {
        VisualElement? anchor = _contentView ?? _loadingView ?? _introView ?? _staggerLayout;
        if (anchor?.Dispatcher is null)
        {
            animation().FireAndForget();
            return;
        }

        anchor.Dispatcher.DispatchAsync(async () =>
        {
            if (!UiAnimations.CanAnimate(anchor) && anchor.Window is null)
            {
                await Task.Yield();
            }

            await animation();
        }).FireAndForget();
    }

    private async Task HandleLoadingStateAsync()
    {
        if (_viewModel is null)
        {
            return;
        }

        ResetCancellation();

        if (_viewModel.IsInit && _introView is not null && !_introDismissed && _introView.IsVisible)
        {
            _introDismissed = true;
            await UiAnimations.SafeHideAsync(_introView, cancellationToken: _cts!.Token);
            return;
        }

        if (_viewModel.IsDone && !_contentRevealed)
        {
            _contentRevealed = true;

            if (_contentView is not null && _contentView.IsVisible)
            {
                await UiAnimations.SafeRevealPremiumAsync(_contentView, cancellationToken: _cts!.Token);
            }

            if (_staggerLayout is not null && _staggerLayout.IsVisible)
            {
                await UiAnimations.RevealChildrenStaggeredAsync(_staggerLayout, cancellationToken: _cts!.Token);
            }
        }
        else if (_viewModel.IsInit)
        {
            _contentRevealed = false;

            if (_contentView is not null)
            {
                UiAnimations.ResetVisualState(_contentView);
            }
        }
        else if (_viewModel.IsCreated)
        {
            _contentRevealed = false;
            _introDismissed = false;
            UiAnimations.ResetVisualState(_introView);
            UiAnimations.ResetVisualState(_contentView);
        }
    }

    private void ResetCancellation()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
    }

    public void Dispose()
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
