using PsychologyApp.Presentation.Widgets.TechniqueBodies;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

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
