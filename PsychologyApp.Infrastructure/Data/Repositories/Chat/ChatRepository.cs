using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Repositories.Base;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.Data.Repositories.Chat;

public sealed class ChatRepository(IDbConnectionFactory connectionFactory, IOptions<AppSettings> settings)
    : SqliteRepositoryBase(connectionFactory, settings), IChatRepository
{
    public async Task<long> CreateSessionAsync(string title, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
            ChatSql.InsertSession,
            new { Title = title, Now = ToIso(nowUtc) },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<ChatSessionDTO>> GetSessionsAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<SessionRow> rows = await connection.QueryAsync<SessionRow>(DapperCommandFactory.Create(
            ChatSql.SelectSessions,
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
        return rows.Select(ToDto).ToList();
    }

    public async Task<ChatSessionDTO?> GetSessionAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        SessionRow? row = await connection.QuerySingleOrDefaultAsync<SessionRow>(DapperCommandFactory.Create(
            ChatSql.SelectSession,
            new { SessionId = sessionId },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
        return row is null ? null : ToDto(row);
    }

    public async Task UpdateSessionAsync(ChatSessionDTO session, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ChatSql.UpdateSession,
            new
            {
                SessionId = session.Id,
                session.Title,
                UpdatedAt = ToIso(session.UpdatedAt),
                session.Emotion,
                session.Theme,
                session.FirstIntensity,
                session.LastIntensity,
                session.StateJson
            },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
    }

    public async Task DeleteSessionAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction = connection.BeginTransaction();
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ChatSql.DeleteMessages, new { SessionId = sessionId }, transaction, CommandTimeoutSeconds, cancellationToken));
        await connection.ExecuteAsync(DapperCommandFactory.Create(
            ChatSql.DeleteSession, new { SessionId = sessionId }, transaction, CommandTimeoutSeconds, cancellationToken));
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<long> AddMessageAsync(ChatMessageDTO message, CancellationToken cancellationToken = default) =>
        (await AddMessagesAsync([message], cancellationToken))[0];

    public async Task<IReadOnlyList<long>> AddMessagesAsync(IReadOnlyList<ChatMessageDTO> messages, CancellationToken cancellationToken = default)
    {
        if (messages.Count == 0)
        {
            return [];
        }

        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        await using SqliteTransaction transaction = connection.BeginTransaction();
        List<long> ids = new(messages.Count);
        foreach (ChatMessageDTO message in messages)
        {
            ids.Add(await connection.ExecuteScalarAsync<long>(DapperCommandFactory.Create(
                ChatSql.InsertMessage,
                new
                {
                    message.SessionId,
                    Role = (int)message.Role,
                    message.Text,
                    CreatedAt = ToIso(message.CreatedAt),
                    QuickRepliesJson = ChatQuickReplyJson.Serialize(message.QuickReplies)
                },
                transaction,
                CommandTimeoutSeconds,
                cancellationToken)));
        }

        await transaction.CommitAsync(cancellationToken);
        return ids;
    }

    public async Task<IReadOnlyList<ChatMessageDTO>> GetMessagesAsync(long sessionId, CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = await OpenConnectionAsync(cancellationToken);
        IEnumerable<MessageRow> rows = await connection.QueryAsync<MessageRow>(DapperCommandFactory.Create(
            ChatSql.SelectMessages,
            new { SessionId = sessionId },
            commandTimeout: CommandTimeoutSeconds,
            cancellationToken: cancellationToken));
        return rows.Select(r => new ChatMessageDTO
        {
            Id = r.MessageId,
            SessionId = r.SessionId,
            Role = (ChatRole)r.Role,
            Text = r.Text,
            CreatedAt = FromIso(r.CreatedAt),
            QuickReplies = ChatQuickReplyJson.Deserialize(r.QuickRepliesJson)
        }).ToList();
    }

    private static ChatSessionDTO ToDto(SessionRow r) => new()
    {
        Id = r.SessionId,
        Title = r.Title,
        CreatedAt = FromIso(r.CreatedAt),
        UpdatedAt = FromIso(r.UpdatedAt),
        Emotion = r.Emotion,
        Theme = r.Theme,
        FirstIntensity = r.FirstIntensity,
        LastIntensity = r.LastIntensity,
        StateJson = r.StateJson,
        Preview = r.Preview,
        MessageCount = r.MessageCount
    };

    private static string ToIso(DateTime value) => value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

    private static DateTime FromIso(string value) =>
        DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).ToUniversalTime();

    private sealed class SessionRow
    {
        public long SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string? Emotion { get; set; }
        public string? Theme { get; set; }
        public int? FirstIntensity { get; set; }
        public int? LastIntensity { get; set; }
        public string? StateJson { get; set; }
        public string? Preview { get; set; }
        public int MessageCount { get; set; }
    }

    private sealed class MessageRow
    {
        public long MessageId { get; set; }
        public long SessionId { get; set; }
        public int Role { get; set; }
        public string Text { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string? QuickRepliesJson { get; set; }
    }
}
