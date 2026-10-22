using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// The icon and tile colours of a test card, by what the test is about, so the list can be read at a glance instead of
/// showing one identical icon on every card. The colours are the practice groups' (<see cref="TechniqueFlavors"/>):
/// the body for sleep, eating and body symptoms, the mind for anxiety and stress, the heart for mood, self-worth and
/// trauma, action for anger and the colour test. A test not listed keeps the plain accent tile.
/// </summary>
public static class TestCardLook
{
    public const string DefaultIcon = "Quiz";

    public static (string Icon, TechniqueFlavor Flavor) For(string? testId) => testId switch
    {
        "beck" or "phq9" or "phq2" or "hads_d" => ("Cloud", TechniqueFlavor.Heart),
        "rses" => ("SelfImprovement", TechniqueFlavor.Heart),
        "who5" => ("Spa", TechniqueFlavor.Heart),
        "pcptsd5" or "pcl5" => ("Shield", TechniqueFlavor.Heart),
        "gad7" or "gad2" or "hads_a" => ("Air", TechniqueFlavor.Mind),
        "pss10" => ("Bolt", TechniqueFlavor.Mind),
        "k10" => ("Thunderstorm", TechniqueFlavor.Mind),
        "heck_hess" => ("Psychology", TechniqueFlavor.Mind),
        "isi" => ("Bedtime", TechniqueFlavor.Body),
        "ess" => ("Snooze", TechniqueFlavor.Body),
        "phq15" => ("MonitorHeart", TechniqueFlavor.Body),
        "scoff" => ("Restaurant", TechniqueFlavor.Body),
        "swls" => ("WbSunny", TechniqueFlavor.Action),
        "pochebut" or "haer" => ("Whatshot", TechniqueFlavor.Action),
        "luscher_brief" or "luscher_standard" => ("Palette", TechniqueFlavor.Action),
        _ => (DefaultIcon, TechniqueFlavor.None)
    };
}
