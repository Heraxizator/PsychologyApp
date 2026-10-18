using PsychologyApp.Application.Practice;

namespace PsychologyApp.Presentation.Common;

/// <summary>
/// One soft colour per emotion family for the emotion palette, with a text colour that stays readable on it in each theme (tested).
/// The hues follow the usual emotion-wheel associations, kept pale so a page of feelings does not look alarming.
/// </summary>
public static class EmotionColors
{
    public static (string Fill, string Text) For(EmotionFamily family, bool dark) => (family, dark) switch
    {
        (EmotionFamily.Fear, false) => ("#E4E0F7", "#4B3A9A"),
        (EmotionFamily.Fear, true) => ("#2E2850", "#C4B8F5"),
        (EmotionFamily.Sadness, false) => ("#DCE8FA", "#2656A8"),
        (EmotionFamily.Sadness, true) => ("#1F3050", "#8DB4F2"),
        (EmotionFamily.Anger, false) => ("#FADBD6", "#A5321F"),
        (EmotionFamily.Anger, true) => ("#4A2420", "#F2A194"),
        (EmotionFamily.Shame, false) => ("#F8DDE5", "#A93660"),
        (EmotionFamily.Shame, true) => ("#47242F", "#F0A0B9"),
        (EmotionFamily.Disgust, false) => ("#E3EED5", "#4F6B1F"),
        (EmotionFamily.Disgust, true) => ("#2C3A1E", "#B7D68A"),
        (EmotionFamily.Joy, false) => ("#FBEFC8", "#80590A"),
        (EmotionFamily.Joy, true) => ("#433A1A", "#F2D37A"),
        _ => dark ? ("#2E2E2E", "#EEEEEE") : ("#ECECEC", "#343434")
    };
}
