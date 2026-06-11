using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.UI.Techniques.Bodies;

public partial class CopiedFormBody : ContentView
{
    public CopiedFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PaperItemsStack);
}
