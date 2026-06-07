using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Profile;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class UserPage : ContentPage
{
    private UserViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;
    private bool _profileRevealed;

    public UserPage(IPageViewModelActivator pageViewModelActivator, IUserViewModelFactory userViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => userViewModelFactory.Create(nav));
        _animationHelper = new PageAnimationHelper(_viewModel, QuotesLoading, QuotesCollection);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.InitAsync().FireAndForget();
        _animationHelper?.TryRevealAsync();

        if (!_profileRevealed)
        {
            _profileRevealed = true;
            UiAnimations.RevealChildrenStaggeredAsync(ProfileContent, allowHidden: true).FireAndForget();
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
