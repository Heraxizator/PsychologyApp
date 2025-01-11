using PsychologyApp.Presentation.Modules.Practic.Constructor;

namespace MobileHelperMaui.Views.TechniquePages.ConstructorPages;

public partial class DesignerPage : ContentPage
{
	public DesignerPage(long id)
	{
		InitializeComponent();
        this.BindingContext = new DesignerViewModel(this.Navigation, id);
    }
}