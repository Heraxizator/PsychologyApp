using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.UserProgress;

public sealed class ClinicalCareRepository : SqliteRepositoryBase, IClinicalCareRepository
{
    public ClinicalCareRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
        : base(connectionFactory, settings)
    {
    }

    public async Task SaveRiskAssessmentAsync(RiskAssessmentDTO assessment, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.InsertRiskAssessment,
            new
            {
                AssessedAt = assessment.AssessedAt.ToString("O"),
                assessment.Source,
                assessment.Notes,
                assessment.HasSelfHarmThoughts,
                assessment.HasSevereDisorientation,
                assessment.HasSubstanceRisk,
                assessment.HasSevereInsomnia,
                RiskLevel = ToRiskKey(assessment.RiskLevel)
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        ClinicalRiskRow? row = await connection.QuerySingleOrDefaultAsync<ClinicalRiskRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectLatestRiskAssessment,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
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

        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.DeactivateAllPrograms,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ClinicalCareSql.UpsertActiveProgram,
            new
            {
                ProgramKey = program.ProgramType.ToString(),
                StartedAt = program.StartedAt.ToString("O"),
                program.CurrentWeek,
                IsActive = program.IsActive ? 1 : 0
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        ProgramRow? row = await connection.QuerySingleOrDefaultAsync<ProgramRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectActiveProgram,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
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
                CreatedAt = escalation.CreatedAt.ToString("O"),
                RiskLevel = ToRiskKey(escalation.RiskLevel),
                escalation.TriggerSource,
                escalation.Action,
                escalation.Notes
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(int limit, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<EscalationRow> rows = await connection.QueryAsync<EscalationRow>(DapperCommandFactory.Create(
            ClinicalCareSql.SelectRecentEscalations,
            new { limit },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));

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
        DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);

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
