namespace PsychologyApp.Application.Abstractions.Integration;

public interface IQuotContentProvider
{
    Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default);
}

public sealed record QuotSeed(string Author, string Text, string Theme);
