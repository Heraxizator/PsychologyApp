namespace PsychologyApp.Presentation.Common;

/// <summary>Copy of the "What helps me" page.</summary>
public static partial class AppStrings
{
    public static string InsightsTitle => R(nameof(InsightsTitle));
    public static string InsightsLead => R(nameof(InsightsLead));
    public static string InsightsPracticesTitle => R(nameof(InsightsPracticesTitle));
    public static string InsightsPracticeLine(object arg0, object arg1) => F(nameof(InsightsPracticeLine), arg0, arg1);
    public static string InsightsPracticesEmpty => R(nameof(InsightsPracticesEmpty));
    public static string InsightsTestsTitle => R(nameof(InsightsTestsTitle));
    public static string InsightsTestChange(object arg0, object arg1) => F(nameof(InsightsTestChange), arg0, arg1);
    public static string InsightsTestSingle(object arg0) => F(nameof(InsightsTestSingle), arg0);
    public static string InsightsTrendBetter => R(nameof(InsightsTrendBetter));
    public static string InsightsTrendWorse => R(nameof(InsightsTrendWorse));
    public static string InsightsTrendSame => R(nameof(InsightsTrendSame));
    public static string InsightsRetestHint => R(nameof(InsightsRetestHint));
    public static string InsightsRetest => R(nameof(InsightsRetest));
    public static string InsightsTestsEmpty => R(nameof(InsightsTestsEmpty));
    public static string InsightsMoodTitle => R(nameof(InsightsMoodTitle));
    public static string InsightsMoodUp(object arg0, object arg1) => F(nameof(InsightsMoodUp), arg0, arg1);
    public static string InsightsMoodDown(object arg0, object arg1) => F(nameof(InsightsMoodDown), arg0, arg1);
    public static string InsightsMoodNote => R(nameof(InsightsMoodNote));
    public static string InsightsMoodEmpty => R(nameof(InsightsMoodEmpty));
    public static string InsightsBodyTitle => R(nameof(InsightsBodyTitle));
}
