using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class AlternativeTestPage : ContentPage
{
    public AlternativeTestPage(
        IPageViewModelActivator pageViewModelActivator,
        IAlternativeTestViewModelFactory alternativeTestViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => alternativeTestViewModelFactory.Create(nav));
    }
}
