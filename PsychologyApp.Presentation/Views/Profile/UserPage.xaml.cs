using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Profile;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class UserPage : ContentPage
{
    private UserViewModel? _viewModel;

    public UserPage(IPageViewModelActivator pageViewModelActivator, IUserViewModelFactory userViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => userViewModelFactory.Create(nav));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.RefreshAsync(forceQuotesReload: false).FireAndForget();
    }
}
