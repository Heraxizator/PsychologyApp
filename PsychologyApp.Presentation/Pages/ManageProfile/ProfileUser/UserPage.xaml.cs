using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserPage : ContentPage
{
    private UserViewModel? _viewModel;

    public UserPage(IPageViewModelActivator pageViewModelActivator, IUserViewModelFactory userViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => userViewModelFactory.Create(page));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
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
}
