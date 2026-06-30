using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Shared.Common.Behaviors;

public sealed class PageAnalyticsBehavior : Behavior<ContentPage>
{
    public static readonly BindableProperty TestIdProperty =
        BindableProperty.Create(nameof(TestId), typeof(string), typeof(PageAnalyticsBehavior), null);

    private ContentPage? _page;
    private DateTime _startedAt;

    public string? TestId
    {
        get => (string?)GetValue(TestIdProperty);
        set => SetValue(TestIdProperty, value);
    }

    protected override void OnAttachedTo(ContentPage bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        bindable.Appearing += OnAppearing;
        bindable.Disappearing += OnDisappearing;
    }

    protected override void OnDetachingFrom(ContentPage bindable)
    {
        bindable.Appearing -= OnAppearing;
        bindable.Disappearing -= OnDisappearing;
        _page = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnAppearing(object? sender, EventArgs e) =>
        _startedAt = DateTime.Now;

    private void OnDisappearing(object? sender, EventArgs e) =>
        TrackAsync().FireAndForget();

    private async Task TrackAsync()
    {
        if (_page?.BindingContext is not BaseViewModel viewModel)
        {
            return;
        }

        try
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<IPageAnalyticsService>() is not IPageAnalyticsService analytics)
            {
                return;
            }

            string pageName = viewModel.PageName;
            if (!string.IsNullOrWhiteSpace(TestId))
            {
                pageName = $"{pageName}:{TestId}";
            }

            await analytics.TrackPageVisitAsync(viewModel.ModuleName, pageName, _startedAt);
        }
        catch (Exception ex)
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<ILogger<PageAnalyticsBehavior>>() is ILogger<PageAnalyticsBehavior> logger)
            {
                logger.LogWarning(ex, "Failed to track page analytics.");
            }
        }
    }
}
