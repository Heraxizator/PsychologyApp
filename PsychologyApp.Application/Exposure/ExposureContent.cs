using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

/// <param name="Text">A step as the person might write it.</param>
/// <param name="ExpectedDistress">A typical rating, 0..100; the person adjusts it.</param>
/// <param name="ProvokesBodySensations">Raises the heart rate or breathing on purpose (stairs, coffee): left out when the screening
/// found a body risk (heart disease, asthma, pregnancy...).</param>
public sealed record ExposureStepExample(string Text, int ExpectedDistress, bool ProvokesBodySensations = false);

/// <summary>Ready answers for one kind of fear, so the ladder is built with taps; the person edits and adds their own.</summary>
public sealed record ExposureKindContent(
    FearKind Kind,
    string Title,
    string Example,
    IReadOnlyList<string> FearedOutcomes,
    IReadOnlyList<string> AvoidedSituations,
    IReadOnlyList<string> SafetyBehaviours,
    IReadOnlyList<ExposureStepExample> Steps);

/// <summary>
/// Examples for the fear ladder in every language of the app. Clinical copy: reviewed together with docs/exposure-engine.md
/// before release; every language matches the English lists one to one (tested).
/// </summary>
public static partial class ExposureContent
{
    /// <summary>Short lessons people often draw after a task: one tap instead of writing the sentence.</summary>
    public static IReadOnlyList<string> Lessons(string? language) => language?.ToLowerInvariant() switch
    {
        "ru" => LessonsRu,
        "fr" => LessonsFr,
        "de" => LessonsDe,
        "es" => LessonsEs,
        "pt" => LessonsPt,
        "zh" => LessonsZh,
        _ => LessonsEn
    };

    /// <summary>The kinds of fear in a language by its key ("ru", "en", "fr"…); a language without its own examples gets the English ones.</summary>
    public static IReadOnlyList<ExposureKindContent> Kinds(string? language) => language?.ToLowerInvariant() switch
    {
        "ru" => KindsRu,
        "fr" => KindsFr,
        "de" => KindsDe,
        "es" => KindsEs,
        "pt" => KindsPt,
        "zh" => KindsZh,
        _ => KindsEn
    };

    public static ExposureKindContent For(FearKind kind, string? language) =>
        Kinds(language).FirstOrDefault(content => content.Kind == kind) ?? Kinds(language)[^1];
}
