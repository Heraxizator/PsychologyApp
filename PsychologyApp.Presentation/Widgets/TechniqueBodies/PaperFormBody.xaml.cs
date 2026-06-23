using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TechniqueBodies;

public partial class PaperFormBody : ContentView
{
    public PaperFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PaperItemsStack);
}
