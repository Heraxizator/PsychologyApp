using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TechniqueBodies;

public partial class PolarityFormBody : ContentView
{
    public PolarityFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PolarityItemsStack);
}
