using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class StandardTestPage : ContentPage
{
    public StandardTestPage(
        IPageViewModelActivator pageViewModelActivator,
        IStandardTestViewModelFactory standardTestViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => standardTestViewModelFactory.Create(nav));
    }
}
