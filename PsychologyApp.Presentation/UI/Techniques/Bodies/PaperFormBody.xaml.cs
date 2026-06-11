using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.UI.Techniques.Bodies;

public partial class PaperFormBody : ContentView
{
    public PaperFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PaperItemsStack);
}
