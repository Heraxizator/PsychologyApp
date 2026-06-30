namespace PsychologyApp.Infrastructure.Data.Sql;

internal static class UserProgressSql
{
    internal const string InsertTestResult = """
        INSERT INTO TestResults (TestId, Score, Summary, DetailJson, CompletedAt)
        VALUES (@TestId, @Score, @Summary, @DetailJson, @CompletedAt);
        """;

    internal const string SelectLatestTestResult = """
        SELECT TestResultId, TestId, Score, Summary, DetailJson, CompletedAt
        FROM TestResults
        WHERE TestId = @testId
        ORDER BY TestResultId DESC
        LIMIT 1;
        """;

    internal const string SelectTestResultHistory = """
        SELECT TestResultId, TestId, Score, Summary, DetailJson, CompletedAt
        FROM TestResults
        WHERE TestId = @testId
        ORDER BY TestResultId DESC
        LIMIT @limit;
        """;

    internal const string SelectLatestTestResults = """
        SELECT tr.TestResultId, tr.TestId, tr.Score, tr.Summary, tr.DetailJson, tr.CompletedAt
        FROM TestResults tr
        INNER JOIN (
            SELECT TestId, MAX(TestResultId) AS MaxTestResultId
            FROM TestResults
            WHERE TestId IN @testIds
            GROUP BY TestId
        ) latest ON tr.TestResultId = latest.MaxTestResultId;
        """;

    internal const string SelectTestResultCounts = """
        SELECT TestId, COUNT(*) AS Count
        FROM TestResults
        WHERE TestId IN @testIds
        GROUP BY TestId;
        """;

    internal const string SelectLastTechniqueCompletionDate = """
        SELECT CompletedAt
        FROM Completions
        WHERE CompletionKind = 'technique'
        ORDER BY CompletionId DESC
        LIMIT 1;
        """;

    internal const string CountTestResults = "SELECT COUNT(*) FROM TestResults;";

    internal const string InsertCompletion = """
        INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
        VALUES (@CompletionKind, @ItemKey, @ModuleName, @PageName, @CompletedAt, @DurationSeconds);
        """;

    internal const string CountTechniqueCompletions =
        "SELECT COUNT(*) FROM Completions WHERE CompletionKind = 'technique';";

    internal const string SelectRecentTechniqueCompletions = """
        SELECT CompletionId, CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds
        FROM Completions
        WHERE CompletionKind = 'technique'
        ORDER BY CompletionId DESC
        LIMIT @limit;
        """;

    internal const string SelectCompletionDates = """
        SELECT DISTINCT substr(CompletedAt, 1, 10) AS Day
        FROM Completions
        ORDER BY Day DESC;
        """;

    internal const string SelectLastCompletionForItem = """
        SELECT CompletedAt
        FROM Completions
        WHERE ItemKey = @itemKey
        ORDER BY CompletionId DESC
        LIMIT 1;
        """;

    internal const string SelectLastPracticeDates = """
        SELECT c.ItemKey, c.CompletedAt
        FROM Completions c
        INNER JOIN (
            SELECT ItemKey, MAX(CompletionId) AS MaxCompletionId
            FROM Completions
            WHERE ItemKey IN @itemKeys
            GROUP BY ItemKey
        ) latest ON c.CompletionId = latest.MaxCompletionId;
        """;

    internal const string UpsertSessionDraft = """
        INSERT INTO SessionDrafts (TechniqueKey, PayloadJson, UpdatedAt)
        VALUES (@techniqueKey, @payloadJson, @updatedAt)
        ON CONFLICT(TechniqueKey) DO UPDATE SET
            PayloadJson = excluded.PayloadJson,
            UpdatedAt = excluded.UpdatedAt;
        """;

    internal const string SelectSessionDraft =
        "SELECT PayloadJson FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;";

    internal const string SelectSessionDraftKeys =
        "SELECT TechniqueKey FROM SessionDrafts WHERE TechniqueKey IN @techniqueKeys;";

    internal const string DeleteSessionDraft =
        "DELETE FROM SessionDrafts WHERE TechniqueKey = @techniqueKey;";

    internal const string InsertMoodEntry = """
        INSERT INTO MoodEntries (MoodLevel, Note, RecordedAt)
        VALUES (@MoodLevel, @Note, @RecordedAt);
        """;

    internal const string InsertMoodCompletion = """
        INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
        VALUES ('mood', 'mood', @moduleName, @pageName, @completedAt, 0);
        """;

    internal const string SelectRecentMoods = """
        SELECT MoodEntryId, MoodLevel, Note, RecordedAt
        FROM MoodEntries
        ORDER BY MoodEntryId DESC
        LIMIT @limit;
        """;
}
