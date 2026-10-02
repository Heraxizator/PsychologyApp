namespace PsychologyApp.Infrastructure.Data.Sql;

/// <summary>
/// Every insert is "INSERT … SELECT … WHERE NOT EXISTS (same moment, same content)": it reports 0 affected rows for a row the
/// database already has, which is what makes importing a backup twice harmless.
/// </summary>
internal static class BackupSql
{
    internal const string SelectTechniques = """
        SELECT Number, Date, Header, Description, Subject, Author, Algorithm, Image, IsCompleted
        FROM Techniques
        ORDER BY TechniqueId;
        """;

    internal const string InsertMood = """
        INSERT INTO MoodEntries (MoodLevel, Note, RecordedAt)
        SELECT @MoodLevel, @Note, @RecordedAt
        WHERE NOT EXISTS (
            SELECT 1 FROM MoodEntries
            WHERE RecordedAt = @RecordedAt AND MoodLevel = @MoodLevel AND IFNULL(Note, '') = IFNULL(@Note, ''));
        """;

    internal const string InsertMoodCompletion = """
        INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
        SELECT 'mood', 'mood', 'Practice', 'Mood', @RecordedAt, 0
        WHERE NOT EXISTS (SELECT 1 FROM Completions WHERE CompletionKind = 'mood' AND CompletedAt = @RecordedAt);
        """;

    internal const string InsertTestResult = """
        INSERT INTO TestResults (TestId, Score, Summary, DetailJson, CompletedAt)
        SELECT @TestId, @Score, @Summary, @DetailJson, @CompletedAt
        WHERE NOT EXISTS (SELECT 1 FROM TestResults WHERE TestId = @TestId AND CompletedAt = @CompletedAt);
        """;

    internal const string InsertCompletion = """
        INSERT INTO Completions (CompletionKind, ItemKey, ModuleName, PageName, CompletedAt, DurationSeconds)
        SELECT @CompletionKind, @ItemKey, @ModuleName, @PageName, @CompletedAt, @DurationSeconds
        WHERE NOT EXISTS (
            SELECT 1 FROM Completions
            WHERE CompletionKind = @CompletionKind AND ItemKey = @ItemKey AND CompletedAt = @CompletedAt);
        """;

    internal const string InsertSessionResult = """
        INSERT INTO SessionResults (
            ItemKey, CompletedAt, DurationSeconds, PayloadJson, PreIntensity, PostIntensity, ProgramType, ProgramWeek, Note)
        SELECT
            @ItemKey, @CompletedAt, @DurationSeconds, @PayloadJson, @PreIntensity, @PostIntensity, @ProgramType, @ProgramWeek, @Note
        WHERE NOT EXISTS (SELECT 1 FROM SessionResults WHERE ItemKey = @ItemKey AND CompletedAt = @CompletedAt);
        """;

    internal const string InsertChatSession = """
        INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt, Emotion, Theme, FirstIntensity, LastIntensity, StateJson)
        SELECT @Title, @CreatedAt, @UpdatedAt, @Emotion, @Theme, @FirstIntensity, @LastIntensity, @StateJson
        WHERE NOT EXISTS (SELECT 1 FROM ChatSessions WHERE Title = @Title AND CreatedAt = @CreatedAt);
        """;

    internal const string InsertChatMessage = """
        INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt, QuickRepliesJson)
        VALUES (@SessionId, @Role, @Text, @CreatedAt, @QuickRepliesJson);
        """;

    internal const string InsertChatMemoryIfMissing = """
        INSERT INTO ChatMemory (MemoryKey, MemoryValue) VALUES (@Key, @Value)
        ON CONFLICT(MemoryKey) DO NOTHING;
        """;

    internal const string InsertTechnique = """
        INSERT INTO Techniques (Number, Date, Header, Description, Subject, Author, Algorithm, Image, IsCompleted)
        SELECT @Number, @Date, @Header, @Description, @Subject, @Author, @Algorithm, @Image, @IsCompleted
        WHERE NOT EXISTS (
            SELECT 1 FROM Techniques WHERE Number = @Number AND Date = @Date AND Header = @Header AND Algorithm = @Algorithm);
        """;

    internal const string InsertRiskAssessmentIfNewest = """
        INSERT INTO RiskAssessments (
            AssessedAt, Source, Notes, HasSelfHarmThoughts, HasSevereDisorientation, HasSubstanceRisk, HasSevereInsomnia, RiskLevel)
        SELECT
            @AssessedAt, @Source, @Notes, @HasSelfHarmThoughts, @HasSevereDisorientation, @HasSubstanceRisk, @HasSevereInsomnia, @RiskLevel
        WHERE @AssessedAt > IFNULL((SELECT MAX(AssessedAt) FROM RiskAssessments), '');
        """;

    internal const string InsertProgramIfNoneActive = """
        INSERT INTO TherapyPrograms (ProgramKey, StartedAt, CurrentWeek, IsActive)
        SELECT @ProgramKey, @StartedAt, @CurrentWeek, @IsActive
        WHERE NOT EXISTS (SELECT 1 FROM TherapyPrograms WHERE IsActive = 1)
        ON CONFLICT(ProgramKey) DO NOTHING;
        """;

    internal const string InsertEscalation = """
        INSERT INTO EscalationEvents (CreatedAt, RiskLevel, TriggerSource, Action, Notes)
        SELECT @CreatedAt, @RiskLevel, @TriggerSource, @Action, @Notes
        WHERE NOT EXISTS (
            SELECT 1 FROM EscalationEvents WHERE CreatedAt = @CreatedAt AND TriggerSource = @TriggerSource AND Action = @Action);
        """;
}
