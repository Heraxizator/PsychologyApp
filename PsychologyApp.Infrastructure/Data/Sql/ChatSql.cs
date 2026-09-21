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
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId) AS MessageCount
        FROM ChatSessions s
        ORDER BY s.UpdatedAt DESC, s.SessionId DESC;
        """;

    internal const string SelectSession = """
        SELECT
            s.SessionId, s.Title, s.CreatedAt, s.UpdatedAt, s.Emotion, s.Theme, s.FirstIntensity, s.LastIntensity, s.StateJson,
            (SELECT m.Text FROM ChatMessages m WHERE m.SessionId = s.SessionId ORDER BY m.MessageId DESC LIMIT 1) AS Preview,
            (SELECT COUNT(*) FROM ChatMessages m WHERE m.SessionId = s.SessionId) AS MessageCount
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
}
