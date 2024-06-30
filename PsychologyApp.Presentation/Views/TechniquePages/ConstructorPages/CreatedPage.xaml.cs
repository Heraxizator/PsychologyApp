using MobileHelper.ViewModels.ConstructorViewModels;

namespace MobileHelperMaui.Views.TechniquePages.ConstructorPages;

public partial class CreatedPage : ContentPage
{
    public CreatedPage(long id)
    {
        InitializeComponent();
        this.BindingContext = new CreatedViewModel(this.Navigation, id);
    }
}