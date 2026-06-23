using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record FindProblemContentSnapshot(
    string? Description,
    string? Comment,
    IReadOnlyList<string> Algorithm);

public sealed class FindProblemContentLoader
{
    public async Task<FindProblemContentSnapshot?> LoadAsync(
        string testId,
        ITestCatalogService catalog,
        CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await catalog.GetByIdAsync(testId, cancellationToken);
        if (definition is null)
        {
            return null;
        }

        return new FindProblemContentSnapshot(
            definition.Description,
            definition.Comment,
            definition.Algorithm.ToList());
    }
}
