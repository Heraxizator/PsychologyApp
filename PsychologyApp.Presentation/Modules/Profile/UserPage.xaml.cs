using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class UserPage : ContentPage
{
    public UserPage(IPageViewModelActivator pageViewModelActivator, IUserViewModelFactory userViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => userViewModelFactory.Create(nav));
    }
}
