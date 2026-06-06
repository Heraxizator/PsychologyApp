using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Views.Clean;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel ViewModel;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory)
    {
        InitializeComponent();

        ViewModel = musicPlayerViewModelFactory.Create();

        BindingContext = ViewModel;
    }
}
