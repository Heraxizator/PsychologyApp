using MobileHelper.ViewModels.CleanViewModels;
using PsychologyApp.Presentation.Models;

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

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        Audio item = e.Item as Audio;

        string file = item.File;

        this.ViewModel.ToExecute(file);
    }
}