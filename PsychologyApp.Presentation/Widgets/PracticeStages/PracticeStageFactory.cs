using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>The interactive stage of a practice that happens in the imagination, bound to the words the person wrote for it.</summary>
public static class PracticeStageFactory
{
    public static PracticeStageView? Create(TechniqueId techniqueId)
    {
        TechniqueFlavor flavor = TechniqueFlavors.For(techniqueId);
        PracticeStageView? stage = techniqueId switch
        {
            TechniqueId.Resize => new ShrinkStageView(flavor),
            TechniqueId.Hack => new WhiteSpotStageView(flavor),
            TechniqueId.Check => new PauseStageView(flavor),
            TechniqueId.Future => new TimeTravelStageView(flavor),
            TechniqueId.Paper => new PaperStageView(flavor),
            TechniqueId.Copied => new RepeatStageView(flavor),
            _ => null
        };

        switch (stage)
        {
            case RepeatStageView repeat:
                repeat.SetBinding(PracticeStageView.SourceTextProperty, "Text");
                repeat.SetBinding(RepeatStageView.RepeatCommandProperty, "AddCommand");
                repeat.SetBinding(RepeatStageView.RepeatCountProperty, "RepeatCount");
                break;
            case PaperStageView paper:
                paper.SetBinding(PracticeStageView.SourceTextProperty, "StageText");
                paper.SetBinding(PaperStageView.KeepCommandProperty, "AddCommand");
                break;
            case not null:
                // The first field of these practices is the problem itself.
                stage.SetBinding(PracticeStageView.SourceTextProperty, "Entries[0].Text");
                break;
        }

        return stage;
    }
}
