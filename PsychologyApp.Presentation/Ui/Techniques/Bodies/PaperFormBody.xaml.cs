using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.Ui.Techniques.Bodies;

public partial class PaperFormBody : ContentView
{
    public PaperFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PaperItemsStack);
}
