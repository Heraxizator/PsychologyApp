namespace PsychologyApp.Presentation.Modules.Motivator;

public partial class QuotePage : ContentPage
{
	private QuoteViewModel ViewModel = default!;
	public QuotePage()
	{
		InitializeComponent();

		ViewModel = new QuoteViewModel(Navigation);
		BindingContext = ViewModel;
	}

    private async void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
		await ViewModel.AddFreshQuotsAsync();
    }
}