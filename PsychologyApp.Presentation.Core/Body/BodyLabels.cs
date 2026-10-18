using PsychologyApp.Application.Body;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Body;

/// <summary>What the Body page calls each place, sensation, feeling and part of the day.</summary>
public static class BodyLabels
{
    public static string Area(BodyArea area) => area switch
    {
        BodyArea.Head => AppStrings.BodyAreaHead,
        BodyArea.Face => AppStrings.BodyAreaFace,
        BodyArea.Throat => AppStrings.BodyAreaThroat,
        BodyArea.Shoulders => AppStrings.BodyAreaShoulders,
        BodyArea.Chest => AppStrings.BodyAreaChest,
        BodyArea.Stomach => AppStrings.BodyAreaStomach,
        BodyArea.Back => AppStrings.BodyAreaBack,
        BodyArea.Arms => AppStrings.BodyAreaArms,
        BodyArea.Legs => AppStrings.BodyAreaLegs,
        BodyArea.WholeBody => AppStrings.BodyAreaWholeBody,
        _ => throw new ArgumentOutOfRangeException(nameof(area), area, null)
    };

    public static string Sensation(BodySensation sensation) => sensation switch
    {
        BodySensation.Tight => AppStrings.BodySensationTight,
        BodySensation.Heavy => AppStrings.BodySensationHeavy,
        BodySensation.Heat => AppStrings.BodySensationHeat,
        BodySensation.Cold => AppStrings.BodySensationCold,
        BodySensation.Lump => AppStrings.BodySensationLump,
        BodySensation.Trembling => AppStrings.BodySensationTrembling,
        BodySensation.Empty => AppStrings.BodySensationEmpty,
        BodySensation.Fluttering => AppStrings.BodySensationFluttering,
        BodySensation.Tingling => AppStrings.BodySensationTingling,
        BodySensation.Pain => AppStrings.BodySensationPain,
        _ => throw new ArgumentOutOfRangeException(nameof(sensation), sensation, null)
    };

    public static string Feeling(BodyFeeling feeling) => feeling switch
    {
        BodyFeeling.Anxiety => AppStrings.BodyFeelingAnxiety,
        BodyFeeling.Fear => AppStrings.BodyFeelingFear,
        BodyFeeling.Anger => AppStrings.BodyFeelingAnger,
        BodyFeeling.Hurt => AppStrings.BodyFeelingHurt,
        BodyFeeling.Sadness => AppStrings.BodyFeelingSadness,
        BodyFeeling.Shame => AppStrings.BodyFeelingShame,
        BodyFeeling.Guilt => AppStrings.BodyFeelingGuilt,
        BodyFeeling.Exhaustion => AppStrings.BodyFeelingExhaustion,
        BodyFeeling.Joy => AppStrings.BodyFeelingJoy,
        BodyFeeling.Unknown => AppStrings.BodyFeelingUnknown,
        _ => throw new ArgumentOutOfRangeException(nameof(feeling), feeling, null)
    };

    public static string Time(PartOfDay part) => part switch
    {
        PartOfDay.Morning => AppStrings.BodyPartMorning,
        PartOfDay.Day => AppStrings.BodyPartDay,
        PartOfDay.Evening => AppStrings.BodyPartEvening,
        PartOfDay.Night => AppStrings.BodyPartNight,
        _ => throw new ArgumentOutOfRangeException(nameof(part), part, null)
    };

    /// <summary>"Lump · Throat": the sensation first, as people say it.</summary>
    public static string Title(BodyArea area, BodySensation sensation) => AppStrings.BodyPatternTitle(Sensation(sensation), Area(area));
}
