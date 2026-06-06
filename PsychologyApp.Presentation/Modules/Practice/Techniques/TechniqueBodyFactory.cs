using PsychologyApp.Presentation.Ui.Techniques.Bodies;

namespace PsychologyApp.Presentation.Modules.Practice.Techniques;

public static class TechniqueBodyFactory
{
    public static View Create(TechniqueUiKind kind) => kind switch
    {
        TechniqueUiKind.Entry => new EntryFormBody(),
        TechniqueUiKind.Polarity => new PolarityFormBody(),
        TechniqueUiKind.Paper => new PaperFormBody(),
        TechniqueUiKind.Copied => new CopiedFormBody(),
        TechniqueUiKind.None => new ContentView(),
        _ => new ContentView()
    };
}
