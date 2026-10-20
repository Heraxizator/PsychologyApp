namespace PsychologyApp.Presentation.Pages.ManageProfile.AppLock;

public partial class AppLockPage : ContentPage
{
    private const int PromptDelayMs = 250;

    private readonly AppLockViewModel _viewModel;
    private bool _prompted;

    public AppLockPage(AppLockViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
        InitializeComponent();
    }

    public Task Unlocked => _viewModel.Unlocked;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_prompted)
        {
            return;
        }

        // The system prompt opens by itself the first time, once the page is on screen; the button is there to try again.
        _prompted = true;
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(PromptDelayMs), () => _viewModel.UnlockCommand.Execute(null));
    }

    // Back must not step past the lock into the app; the phone's Home button still leaves it.
    protected override bool OnBackButtonPressed() => true;
}
