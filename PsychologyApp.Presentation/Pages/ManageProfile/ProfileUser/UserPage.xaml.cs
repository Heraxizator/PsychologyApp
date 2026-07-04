using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserPage : ContentPage
{
    private UserViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;

    public UserPage(IPageViewModelActivator pageViewModelActivator, IUserViewModelFactory userViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => userViewModelFactory.Create(page));
        _animationHelper = new PageAnimationHelper(_viewModel, contentView: ProfileContent);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();

        if (_viewModel is null)
        {
            return;
        }

        if (_viewModel.HasInitialized)
        {
            _viewModel.RefreshAsync(forceQuotesReload: false).FireAndForget();
        }
        else
        {
            _viewModel.EnsureInitializedAsync().FireAndForget();
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
