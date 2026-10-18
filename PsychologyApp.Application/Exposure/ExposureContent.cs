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
/// Examples for the fear ladder in Russian and English. Clinical copy: reviewed together with docs/exposure-engine.md
/// before release; Russian and English lists match one to one (tested).
/// </summary>
public static partial class ExposureContent
{
    /// <summary>Short lessons people often draw after a task: one tap instead of writing the sentence.</summary>
    public static IReadOnlyList<string> Lessons(bool english) => english ? LessonsEn : LessonsRu;

    public static IReadOnlyList<ExposureKindContent> Kinds(bool english) => english ? KindsEn : KindsRu;

    public static ExposureKindContent For(FearKind kind, bool english) =>
        Kinds(english).FirstOrDefault(content => content.Kind == kind) ?? Kinds(english)[^1];
}
