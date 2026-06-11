using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Views.Practice.Techniques;

namespace PsychologyApp.Presentation.Common.Behaviors;

public class TechniqueSessionBehavior : Behavior<ContentPage>
{
    private DateTime _sessionStartedAt;
    private ContentPage? _page;

    protected override void OnAttachedTo(ContentPage bindable)
    {
        base.OnAttachedTo(bindable);
        _page = bindable;
        bindable.Appearing += OnPageAppearing;
        bindable.Disappearing += OnPageDisappearing;
    }

    protected override void OnDetachingFrom(ContentPage bindable)
    {
        bindable.Appearing -= OnPageAppearing;
        bindable.Disappearing -= OnPageDisappearing;
        _page = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnPageAppearing(object? sender, EventArgs e) =>
        _sessionStartedAt = DateTime.Now;

    private void OnPageDisappearing(object? sender, EventArgs e) =>
        _ = TrackSessionAsync();

    private async Task TrackSessionAsync()
    {
        if (_page?.BindingContext is not BaseViewModel viewModel)
        {
            return;
        }

        try
        {
            if (_page is not TechniqueSessionPage sessionPage)
            {
                return;
            }

            await sessionPage.AnalyticsService.TrackPageVisitAsync(viewModel.ModuleName, viewModel.PageName, _sessionStartedAt);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}
