namespace PsychologyApp.Application.Models.Tests;

public static class LuscherNavigationMapper
{
    public static LuscherMode ParseMode(string navigationTarget) => navigationTarget switch
    {
        "StandardTestPage" or "standard" => LuscherMode.Standard,
        "AlternativeTestPage" or "brief" => LuscherMode.Brief,
        _ => LuscherMode.Brief
    };

    public static string ToTestId(LuscherMode mode) => mode switch
    {
        LuscherMode.Standard => TestIds.LuscherStandard,
        LuscherMode.Brief => TestIds.LuscherBrief,
        _ => TestIds.LuscherBrief
    };

    public static TestKind ToKind(LuscherMode mode) => mode switch
    {
        LuscherMode.Standard => TestKind.LuscherStandard,
        LuscherMode.Brief => TestKind.LuscherBrief,
        _ => TestKind.LuscherBrief
    };
}
