using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestsListLoadResult(
    IReadOnlyList<TestItem> Items,
    bool ProgressDeferred);
