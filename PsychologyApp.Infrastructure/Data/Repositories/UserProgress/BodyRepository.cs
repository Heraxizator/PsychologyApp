using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Body;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System.Text.Json.Serialization;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

/// <summary>Body check-ins: one document in AppMetadata (see <see cref="AppMetadataDocumentRepository{T}"/>).</summary>
public sealed class BodyRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    : AppMetadataDocumentRepository<BodyLog>(connectionFactory, settings, MetadataKey, BodyJsonContext.Default.BodyLog), IBodyRepository
{
    public const string MetadataKey = "BodyLog";

    public Task<BodyLog?> GetAsync(CancellationToken cancellationToken = default) => ReadAsync(cancellationToken);

    public Task SaveAsync(BodyLog log, CancellationToken cancellationToken = default) => WriteAsync(log, cancellationToken);
}

/// <summary>Source-generated so the document survives trimming and AOT in release builds.</summary>
[JsonSerializable(typeof(BodyLog))]
internal sealed partial class BodyJsonContext : JsonSerializerContext;
