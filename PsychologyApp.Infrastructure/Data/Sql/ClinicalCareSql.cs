namespace PsychologyApp.Infrastructure.Data.Sql;

internal static class ClinicalCareSql
{
    internal const string InsertRiskAssessment = """
        INSERT INTO RiskAssessments (
            AssessedAt,
            Source,
            Notes,
            HasSelfHarmThoughts,
            HasSevereDisorientation,
            HasSubstanceRisk,
            HasSevereInsomnia,
            RiskLevel)
        VALUES (
            @AssessedAt,
            @Source,
            @Notes,
            @HasSelfHarmThoughts,
            @HasSevereDisorientation,
            @HasSubstanceRisk,
            @HasSevereInsomnia,
            @RiskLevel);
        """;

    internal const string SelectLatestRiskAssessment = """
        SELECT
            RiskAssessmentId,
            AssessedAt,
            Source,
            Notes,
            HasSelfHarmThoughts,
            HasSevereDisorientation,
            HasSubstanceRisk,
            HasSevereInsomnia,
            RiskLevel
        FROM RiskAssessments
        ORDER BY RiskAssessmentId DESC
        LIMIT 1;
        """;

    internal const string DeactivateAllPrograms = """
        UPDATE TherapyPrograms SET IsActive = 0;
        """;

    internal const string UpsertActiveProgram = """
        INSERT INTO TherapyPrograms (ProgramKey, StartedAt, CurrentWeek, IsActive)
        VALUES (@ProgramKey, @StartedAt, @CurrentWeek, @IsActive)
        ON CONFLICT(ProgramKey) DO UPDATE SET
            StartedAt = excluded.StartedAt,
            CurrentWeek = excluded.CurrentWeek,
            IsActive = excluded.IsActive;
        """;

    internal const string SelectActiveProgram = """
        SELECT ProgramKey, StartedAt, CurrentWeek, IsActive
        FROM TherapyPrograms
        WHERE IsActive = 1
        ORDER BY StartedAt DESC
        LIMIT 1;
        """;

    internal const string InsertEscalation = """
        INSERT INTO EscalationEvents (CreatedAt, RiskLevel, TriggerSource, Action, Notes)
        VALUES (@CreatedAt, @RiskLevel, @TriggerSource, @Action, @Notes);
        """;

    internal const string SelectRecentEscalations = """
        SELECT EscalationEventId, CreatedAt, RiskLevel, TriggerSource, Action, Notes
        FROM EscalationEvents
        ORDER BY EscalationEventId DESC
        LIMIT @limit;
        """;
}
