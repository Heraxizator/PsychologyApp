using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Motivator;

namespace PsychologyApp.Presentation.Views.Motivator;

public partial class QuotePage : ContentPage
{
    private QuoteViewModel ViewModel = default!;

    public QuotePage(IPageViewModelActivator pageViewModelActivator, IQuoteViewModelFactory quoteViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, nav => quoteViewModelFactory.Create(nav));
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e) =>
        ViewModel.LoadMoreQuotesCommand.Execute(null);
}
