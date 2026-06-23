using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;

namespace PsychologyApp.Presentation.Shared.Common;

public static class ColourStrings
{
    public static string GetColorName(ColourValue colour)
    {
        if (colour == ColourValue.Black)
        {
            return T("Чёрный", "Black");
        }

        if (colour == ColourValue.Red)
        {
            return T("Красный", "Red");
        }

        if (colour == ColourValue.Brown)
        {
            return T("Коричневый", "Brown");
        }

        if (colour == ColourValue.Yellow)
        {
            return T("Жёлтый", "Yellow");
        }

        if (colour == ColourValue.Green)
        {
            return T("Зелёный", "Green");
        }

        if (colour == ColourValue.Blue)
        {
            return T("Синий", "Blue");
        }

        if (colour == ColourValue.Purple)
        {
            return T("Фиолетовый", "Purple");
        }

        if (colour == ColourValue.Gray)
        {
            return T("Серый", "Gray");
        }

        return colour.ToString();
    }

    public static string GetExplanation(ColourValue colour, ColourMeaningType meaningType)
    {
        if (!UserPreferences.IsEnglish(AppStrings.Language))
        {
            return GetDomainMeaning(colour, meaningType).Explaination!;
        }

        return meaningType == ColourMeaningType.Wanted
            ? GetVotedEn(colour)
            : GetUnvotedEn(colour);
    }

    private static ColourMeaning GetDomainMeaning(ColourValue colour, ColourMeaningType meaningType)
    {
        if (meaningType == ColourMeaningType.Wanted)
        {
            if (colour == ColourValue.Black)
            {
                return ColourMeaning.BlackVoted;
            }

            if (colour == ColourValue.Red)
            {
                return ColourMeaning.RedVoted;
            }

            if (colour == ColourValue.Blue)
            {
                return ColourMeaning.BlueVoted;
            }

            if (colour == ColourValue.Purple)
            {
                return ColourMeaning.PurpleVoted;
            }

            if (colour == ColourValue.Yellow)
            {
                return ColourMeaning.YellowVoted;
            }

            if (colour == ColourValue.Brown)
            {
                return ColourMeaning.BrownVoted;
            }

            if (colour == ColourValue.Green)
            {
                return ColourMeaning.GreenVoted;
            }

            return ColourMeaning.GrayVoted;
        }

        if (colour == ColourValue.Black)
        {
            return ColourMeaning.BlackUnvoted;
        }

        if (colour == ColourValue.Red)
        {
            return ColourMeaning.RedUnvoted;
        }

        if (colour == ColourValue.Blue)
        {
            return ColourMeaning.BlueUnvoted;
        }

        if (colour == ColourValue.Purple)
        {
            return ColourMeaning.PurpleUnvoted;
        }

        if (colour == ColourValue.Yellow)
        {
            return ColourMeaning.YellowUnvoted;
        }

        if (colour == ColourValue.Brown)
        {
            return ColourMeaning.BrownUnvoted;
        }

        if (colour == ColourValue.Green)
        {
            return ColourMeaning.GreenUnvoted;
        }

        return ColourMeaning.GrayUnvoted;
    }

    private static string GetVotedEn(ColourValue colour)
    {
        if (colour == ColourValue.Black)
        {
            return "Negativity, rejection of pleasure, and aggression fill your mind and body. You feel hostile and may burst into rage at any moment. You are close to destroying yourself or your relationships.";
        }

        if (colour == ColourValue.Red)
        {
            return "You are emotionally aroused. Your mood is elevated. You strive for achievement and success. You may be pushy, even aggressive.";
        }

        if (colour == ColourValue.Blue)
        {
            return "You seek harmony, trust, understanding, and empathy. You feel emotionally comfortable, calm, gentle, and dreamy. You are open to talking with friends.";
        }

        if (colour == ColourValue.Purple)
        {
            return "You are flirtatious and seek some kind of romantic or sexual intrigue. You want to please, gain support, or receive a compliment. Your mood is even but not calm.";
        }

        if (colour == ColourValue.Yellow)
        {
            return "Optimism fills your soul and makes your heart beat faster. You are relaxed and full of dreams of good fortune. You are ready for change and full release from ties or obligations.";
        }

        if (colour == ColourValue.Brown)
        {
            return "You are tired and seek rest and emotional stability. You are psychologically exhausted and hungry for supportive relationships. Deep down you fear something and do not feel safe. You need sensual satisfaction.";
        }

        if (colour == ColourValue.Green)
        {
            return "You are self-confident, even overconfident. This is a peak of your strength and self-respect. You can do much and want to dominate in communication. Or conversely, you have taken a defensive stance.";
        }

        return "You are looking for a shoulder to lean on. You want to hide from everything heavy in your life and find emotional peace and refuge. You mimic and mask your true feelings with feigned indifference.";
    }

    private static string GetUnvotedEn(ColourValue colour)
    {
        if (colour == ColourValue.Black)
        {
            return "Outwardly you are calm and confident. However, you have driven aggression deep inside and moved onto denial and self-punishment.";
        }

        if (colour == ColourValue.Red)
        {
            return "You are constantly irritated and overstimulated. You are under deep stress. Sometimes you feel drained or even exhausted.";
        }

        if (colour == ColourValue.Blue)
        {
            return "You are anxious. A close relationship may have recently ended. You feel lonely and upset.";
        }

        if (colour == ColourValue.Purple)
        {
            return "You want to stay unnoticed and hide from excessive attention. Modesty, control of feelings, and restraint characterize you right now.";
        }

        if (colour == ColourValue.Yellow)
        {
            return "You are disappointed to the point of despair. You are distrustful and suspicious. Your emotional state is unstable: ups and sharp drops.";
        }

        if (colour == ColourValue.Brown)
        {
            return "You are tense like a drawn string. You deny all your emotional and physical needs. You run from weakness, restricting yourself in everything. You need sensual satisfaction.";
        }

        if (colour == ColourValue.Green)
        {
            return "You are frustrated by lack of attention and respect from your partner. You feel humiliated, hurt, wounded, and drained. You have no strength left to resist.";
        }

        return "You are proactive as never before. You are fully engaged in the here-and-now. You are sociable, moderately cheerful, and resourceful. You have a goal and feel confident calm about tomorrow. You seem to have found your purpose.";
    }

    private static string T(string russian, string english) =>
        UserPreferences.IsEnglish(AppStrings.Language) ? english : russian;
}
