namespace PsychologyApp.Infrastructure.Data.Sql;

internal static class ChatSql
{
    internal const string InsertSession = """
        INSERT INTO ChatSessions (Title, CreatedAt, UpdatedAt)
        VALUES (@Title, @Now, @Now);
        SELECT last_insert_rowid();
        """;

    internal const string SelectSessions = """
        SELECT
            s.SessionId, s.Title, s.CreatedAt, s.UpdatedAt, s.Emotion, s.Theme, s.FirstIntensity, s.LastIntensity, s.StateJson,
            (SELECT m.Text FROM ChatMessages m WHERE m.SessionId = s.SessionId ORDER BY m.MessageId DESC LIMIT 1) AS Preview,
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId) AS MessageCount,
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId AND m.Role = 0) AS UserMessageCount
        FROM ChatSessions s
        ORDER BY s.UpdatedAt DESC, s.SessionId DESC;
        """;

    internal const string SelectSession = """
        SELECT
            s.SessionId, s.Title, s.CreatedAt, s.UpdatedAt, s.Emotion, s.Theme, s.FirstIntensity, s.LastIntensity, s.StateJson,
            (SELECT m.Text FROM ChatMessages m WHERE m.SessionId = s.SessionId ORDER BY m.MessageId DESC LIMIT 1) AS Preview,
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId) AS MessageCount,
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId AND m.Role = 0) AS UserMessageCount
        FROM ChatSessions s
        WHERE s.SessionId = @SessionId;
        """;

    internal const string UpdateSession = """
        UPDATE ChatSessions
        SET Title = @Title, UpdatedAt = @UpdatedAt, Emotion = @Emotion, Theme = @Theme,
            FirstIntensity = @FirstIntensity, LastIntensity = @LastIntensity, StateJson = @StateJson
        WHERE SessionId = @SessionId;
        """;

    internal const string DeleteMessages = "DELETE FROM ChatMessages WHERE SessionId = @SessionId;";

    internal const string DeleteSession = "DELETE FROM ChatSessions WHERE SessionId = @SessionId;";

    internal const string InsertMessage = """
        INSERT INTO ChatMessages (SessionId, Role, Text, CreatedAt, QuickRepliesJson)
        VALUES (@SessionId, @Role, @Text, @CreatedAt, @QuickRepliesJson);
        SELECT last_insert_rowid();
        """;

    internal const string SelectMessages = """
        SELECT MessageId, SessionId, Role, Text, CreatedAt, QuickRepliesJson
        FROM ChatMessages
        WHERE SessionId = @SessionId
        ORDER BY MessageId ASC;
        """;

    internal const string SelectMemory = "SELECT MemoryKey AS Key, MemoryValue AS Value FROM ChatMemory;";

    internal const string UpsertMemory = """
        INSERT INTO ChatMemory (MemoryKey, MemoryValue) VALUES (@Key, @Value)
        ON CONFLICT(MemoryKey) DO UPDATE SET MemoryValue = excluded.MemoryValue;
        """;

    internal const string IncrementMemory = """
        INSERT INTO ChatMemory (MemoryKey, MemoryValue) VALUES (@Key, '1')
        ON CONFLICT(MemoryKey) DO UPDATE SET MemoryValue = CAST(CAST(MemoryValue AS INTEGER) + 1 AS TEXT);
        """;

    internal const string DeleteMemoryKey = "DELETE FROM ChatMemory WHERE MemoryKey = @Key;";

    internal const string ClearMemory = "DELETE FROM ChatMemory;";

    internal const string DeleteAllMessages = "DELETE FROM ChatMessages;";

    internal const string DeleteAllSessions = "DELETE FROM ChatSessions;";
}
