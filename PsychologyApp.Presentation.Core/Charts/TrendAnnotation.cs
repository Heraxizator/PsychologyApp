namespace PsychologyApp.Presentation.Core.Charts;

/// <summary>Something that happened between two readings (a practice) that the chart marks on the line.</summary>
public sealed record TrendAnnotation(DateTime OccurredAt);

/// <summary>A mark placed on the chart: where along the line (0..1 across the plot), at what height (0..1), and whether the next reading was better.</summary>
public readonly record struct PlacedAnnotation(float X, float Y, bool Helped);
