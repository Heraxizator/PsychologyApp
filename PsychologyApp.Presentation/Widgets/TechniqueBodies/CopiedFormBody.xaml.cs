using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.TechniqueBodies;

public partial class CopiedFormBody : ContentView
{
    public CopiedFormBody()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e) => DynamicListReveal.Attach(PaperItemsStack);
}
