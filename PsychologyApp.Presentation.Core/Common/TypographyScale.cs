namespace PsychologyApp.Presentation.Common;

/// <summary>Font sizes for the three text-size settings. Nothing the person reads is smaller than 12: a caption follows the body down but stops there.</summary>
public static class TypographyScale
{
    public const double MinimumReadable = 12;

    public static (double PageTitle, double Section, double Body, double Caption) For(string sizeKey)
    {
        (double pageTitle, double section, double body) = sizeKey switch
        {
            "large" => (22.0, 20.0, 16.0),
            "small" => (18.0, 16.0, 13.0),
            _ => (20.0, 18.0, 15.0)
        };

        return (pageTitle, section, body, Math.Max(body - 2, MinimumReadable));
    }
}
