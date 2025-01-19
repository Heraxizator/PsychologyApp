using MobileHelper.ViewModels.CleanViewModels;
using PsychologyApp.Presentation.Modules.Cleaner;

namespace MobileHelperMaui.Views.CleanPages;

public partial class MusicPlayerPage : ContentPage
{
    private MusicPlayerViewModel ViewModel;
	public MusicPlayerPage()
	{
		InitializeComponent();

        var viewModel = new MusicPlayerViewModel();

        this.ViewModel = viewModel;

        this.BindingContext = viewModel;
	}

    private async void Musics_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        Audio? audio = e.Item as Audio;

        if (audio is null)
        {
            return;
        }

        await ViewModel.PlayAudioAsync(audio.URL!);
    }
}