using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Views.Clean;

public partial class MusicPlayerPage : ContentPage
{
    private readonly MusicPlayerViewModel ViewModel;
    private PageAnimationHelper? _animationHelper;

    public MusicPlayerPage(IMusicPlayerViewModelFactory musicPlayerViewModelFactory)
    {
        InitializeComponent();

        ViewModel = musicPlayerViewModelFactory.Create();

        BindingContext = ViewModel;
        _animationHelper = new PageAnimationHelper(ViewModel, Progress, Musics, introView: IntroPrompt);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
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
