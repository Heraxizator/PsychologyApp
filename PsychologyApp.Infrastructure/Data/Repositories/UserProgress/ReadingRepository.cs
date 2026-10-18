using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System.Text.Json.Serialization;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

/// <summary>
/// What the person reads and the courses they go through: one document in AppMetadata (see
/// <see cref="AppMetadataDocumentRepository{T}"/>). Course answers are the person's own words, which is why this is not in Preferences.
/// </summary>
public sealed class ReadingRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    : AppMetadataDocumentRepository<ReadingState>(connectionFactory, settings, MetadataKey, ReadingJsonContext.Default.ReadingState), IReadingRepository
{
    public const string MetadataKey = "ReadingState";

    public Task<ReadingState?> GetAsync(CancellationToken cancellationToken = default) => ReadAsync(cancellationToken);

    public Task SaveAsync(ReadingState state, CancellationToken cancellationToken = default) => WriteAsync(state, cancellationToken);
}

/// <summary>Source-generated so the document survives trimming and AOT in release builds.</summary>
[JsonSerializable(typeof(ReadingState))]
internal sealed partial class ReadingJsonContext : JsonSerializerContext;
