using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Domain.Exposure;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using System.Text.Json.Serialization;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

/// <summary>The fear ladder: one document in AppMetadata, like the safety plan (see <see cref="AppMetadataDocumentRepository{T}"/>).</summary>
public sealed class ExposureRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    : AppMetadataDocumentRepository<ExposurePlan>(connectionFactory, settings, MetadataKey, ExposureJsonContext.Default.ExposurePlan), IExposureRepository
{
    public const string MetadataKey = "ExposurePlan";

    public Task<ExposurePlan?> GetAsync(CancellationToken cancellationToken = default) => ReadAsync(cancellationToken);

    public Task SaveAsync(ExposurePlan plan, CancellationToken cancellationToken = default) => WriteAsync(plan, cancellationToken);

    public Task DeleteAsync(CancellationToken cancellationToken = default) => RemoveAsync(cancellationToken);
}

/// <summary>
/// Source-generated so the plan survives trimming and AOT in release builds. Enums are stored by name: a value added or reordered
/// later must not change what an existing record means.
/// </summary>
[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(ExposurePlan))]
internal sealed partial class ExposureJsonContext : JsonSerializerContext;
