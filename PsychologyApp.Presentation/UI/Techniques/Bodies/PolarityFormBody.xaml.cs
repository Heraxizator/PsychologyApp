using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.UI.Techniques.Bodies;

public partial class PolarityFormBody : ContentView
{
    public PolarityFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PolarityItemsStack);
}
