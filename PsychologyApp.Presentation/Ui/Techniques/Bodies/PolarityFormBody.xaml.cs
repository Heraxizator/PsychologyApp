using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.Ui.Techniques.Bodies;

public partial class PolarityFormBody : ContentView
{
    public PolarityFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PolarityItemsStack);
}
