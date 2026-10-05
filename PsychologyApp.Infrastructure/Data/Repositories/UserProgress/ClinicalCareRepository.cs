using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.ClinicalCare;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;
using System.Text.Json;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

public sealed class ClinicalCareRepository : SqliteRepositoryBase, IClinicalCareRepository
{
    private const string SafetyPlanMetadataKey = "SafetyPlan";

    public ClinicalCareRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, settings)
    {
    }

    public async Task<SafetyPlanDTO?> GetSafetyPlanAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        string? json = await connection.ExecuteScalarAsync<string?>(DapperCommandFactory.Create(
            "SELECT Value FROM AppMetadata WHERE Key = @key;",
            new { key = SafetyPlanMetadataKey },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<SafetyPlanDTO>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task SaveSafetyPlanAsync(SafetyPlanDTO plan, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(plan);
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            """
            INSERT INTO AppMetadata (Key, Value)
            VALUES (@key, @value)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            """,
            new { key = SafetyPlanMetadataKey, value = json },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task SaveRiskAssessmentAsync(RiskAssessmentDTO assessment, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.InsertRiskAssessment,
            new
            {
                AssessedAt = SqliteTime.ToIso(assessment.AssessedAt),
                assessment.Source,
                assessment.Notes,
                assessment.HasSelfHarmThoughts,
                assessment.HasSevereDisorientation,
                assessment.HasSubstanceRisk,
                assessment.HasSevereInsomnia,
                RiskLevel = ToRiskKey(assessment.RiskLevel)
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        ClinicalRiskRow? row = await connection.QuerySingleOrDefaultAsync<ClinicalRiskRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectLatestRiskAssessment,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
        return row is null
            ? null
            : new RiskAssessmentDTO
            {
                RiskAssessmentId = row.RiskAssessmentId,
                AssessedAt = ParseUtcDateTime(row.AssessedAt),
                Source = row.Source,
                Notes = row.Notes,
                HasSelfHarmThoughts = row.HasSelfHarmThoughts,
                HasSevereDisorientation = row.HasSevereDisorientation,
                HasSubstanceRisk = row.HasSubstanceRisk,
                HasSevereInsomnia = row.HasSevereInsomnia,
                RiskLevel = ParseRiskLevel(row.RiskLevel)
            };
    }

    public async Task UpsertActiveProgramAsync(TherapyProgramStateDTO program, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        // Deactivate-then-upsert is one change: a failure between the two must not leave the person with no active program.
        await using SqliteTransaction transaction =
            (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.DeactivateAllPrograms,
            transaction: transaction,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);

        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.UpsertActiveProgram,
            new
            {
                ProgramKey = program.ProgramType.ToString(),
                StartedAt = SqliteTime.ToIso(program.StartedAt),
                program.CurrentWeek,
                IsActive = program.IsActive ? 1 : 0
            },
            transaction,
            CommandTimeoutSeconds,
            cancellationToken)).ConfigureAwait(false);

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        ProgramRow? row = await connection.QuerySingleOrDefaultAsync<ProgramRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectActiveProgram,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
        if (row is null)
        {
            return null;
        }

        if (!Enum.TryParse<TherapyProgramType>(row.ProgramKey, true, out TherapyProgramType programType))
        {
            programType = TherapyProgramType.Stress;
        }

        return new TherapyProgramStateDTO
        {
            ProgramType = programType,
            StartedAt = ParseUtcDateTime(row.StartedAt),
            CurrentWeek = row.CurrentWeek <= 0 ? 1 : row.CurrentWeek,
            IsActive = row.IsActive
        };
    }

    public async Task SaveEscalationEventAsync(EscalationEventDTO escalation, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.InsertEscalation,
            new
            {
                CreatedAt = SqliteTime.ToIso(escalation.CreatedAt),
                RiskLevel = ToRiskKey(escalation.RiskLevel),
                escalation.TriggerSource,
                escalation.Action,
                escalation.Notes
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenReadConnectionAsync(cancellationToken);
        IEnumerable<EscalationRow> rows = await connection.QueryAsync<EscalationRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectRecentEscalations,
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken)).ConfigureAwait(false);

        return rows.Select(row => new EscalationEventDTO
        {
            EscalationEventId = row.EscalationEventId,
            CreatedAt = ParseUtcDateTime(row.CreatedAt),
            RiskLevel = ParseRiskLevel(row.RiskLevel),
            TriggerSource = row.TriggerSource,
            Action = row.Action,
            Notes = row.Notes
        }).ToList();
    }

    private static string ToRiskKey(RiskLevel level) => level.ToString().ToLowerInvariant();

    private static RiskLevel ParseRiskLevel(string key) =>
        key.ToLowerInvariant() switch
        {
            "red" => RiskLevel.Red,
            "amber" => RiskLevel.Amber,
            _ => RiskLevel.Green
        };

    private static DateTime ParseUtcDateTime(string value) =>
        SqliteTime.FromIso(value);

    private sealed class ClinicalRiskRow
    {
        public long RiskAssessmentId { get; init; }
        public string AssessedAt { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string Notes { get; init; } = string.Empty;
        public bool HasSelfHarmThoughts { get; init; }
        public bool HasSevereDisorientation { get; init; }
        public bool HasSubstanceRisk { get; init; }
        public bool HasSevereInsomnia { get; init; }
        public string RiskLevel { get; init; } = string.Empty;
    }

    private sealed class ProgramRow
    {
        public string ProgramKey { get; init; } = string.Empty;
        public string StartedAt { get; init; } = string.Empty;
        public int CurrentWeek { get; init; }
        public bool IsActive { get; init; }
    }

    private sealed class EscalationRow
    {
        public long EscalationEventId { get; init; }
        public string CreatedAt { get; init; } = string.Empty;
        public string RiskLevel { get; init; } = string.Empty;
        public string TriggerSource { get; init; } = string.Empty;
        public string Action { get; init; } = string.Empty;
        public string Notes { get; init; } = string.Empty;
    }
}
