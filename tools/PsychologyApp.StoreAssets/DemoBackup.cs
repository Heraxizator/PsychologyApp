using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.Models;

namespace PsychologyApp.StoreAssets;

/// <summary>
/// A believable month of use as a backup file: moods with short notes, a daily practice for the last week, two chats whose companion lines come from the real
/// dialogue engine (so the screenshots show what the app really says), and the tension before and after practices. Imported into a debug build on the
/// phone, it fills the screens without typing anything. Everything is invented; nothing comes from a real person.
/// </summary>
public static class DemoBackup
{
    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    // Mood level 1..5 for the last 30 days, oldest first: a hard stretch that slowly eases, with ordinary dips.
    private static readonly int[] Levels = [2, 2, 3, 2, 3, 3, 2, 3, 4, 3, 3, 2, 3, 4, 4, 3, 4, 4, 3, 4, 5, 4, 4, 3, 4, 4, 5, 4, 4, 5];

    private static readonly Dictionary<int, string> Notes = new()
    {
        [29] = "Плохо спалось, на работе всё валилось из рук",
        [27] = "Поссорились с мамой, до сих пор обидно",
        [24] = "Прогулка вечером немного отвлекла",
        [21] = "Весь день тревожно перед отчётом",
        [18] = "Разговор с подругой помог, стало легче",
        [15] = "Выспаться удалось, спокойный день",
        [12] = "Дыхательная практика помогла успокоиться",
        [9] = "Усталость, но вечером получилась зарядка",
        [6] = "Хороший разговор с коллегой на работе",
        [4] = "Прогулка в парке подняла настроение",
        [2] = "Спокойный день, получилось отдохнуть",
        [0] = "Сегодня заметно легче"
    };

    private static readonly (string Key, string Title)[] Practices =
    [
        ("Breathing", "Квадратное дыхание"),
        ("Grounding", "Заземление 5-4-3-2-1"),
        ("Observer", "Позиция наблюдателя"),
        ("ThoughtRecord", "Запись мысли"),
        ("SmallStep", "Один маленький шаг"),
        ("SelfCompassion", "Добрые слова себе"),
        ("Anchor", "Якорь ресурса")
    ];

    public static AppBackupDTO Build(DateTime nowUtc)
    {
        DateTime today = nowUtc.ToLocalTime().Date;
        List<MoodEntryDTO> moods = [];
        for (int i = 0; i < Levels.Length; i++)
        {
            int daysAgo = Levels.Length - 1 - i;
            DateTime day = today.AddDays(-daysAgo);
            moods.Add(new MoodEntryDTO
            {
                MoodLevel = Levels[i],
                Note = Notes.GetValueOrDefault(daysAgo),
                RecordedAt = day.AddHours(9).AddMinutes(10 + i % 7 * 3).ToUniversalTime()
            });

            if (i % 3 == 1)
            {
                moods.Add(new MoodEntryDTO { MoodLevel = Math.Clamp(Levels[i] + (i % 2 == 0 ? 1 : -1), 1, 5), RecordedAt = day.AddHours(21).AddMinutes(5).ToUniversalTime() });
            }
        }

        List<CompletionDTO> completions = [];
        List<SessionResultDTO> sessions = [];
        for (int daysAgo = 29; daysAgo >= 0; daysAgo--)
        {
            // Every other day at first, every day for the last eight days: a real streak.
            if (daysAgo >= 8 && daysAgo % 2 == 1)
            {
                continue;
            }

            (string key, string title) = Practices[(29 - daysAgo) % Practices.Length];
            DateTime at = today.AddDays(-daysAgo).AddHours(daysAgo % 3 == 0 ? 19 : 8).AddMinutes(20 + daysAgo % 5 * 4).ToUniversalTime();
            int seconds = 180 + daysAgo % 4 * 60;
            completions.Add(new CompletionDTO
            {
                CompletionKind = "technique",
                ItemKey = key,
                ModuleName = "Практик",
                PageName = title,
                CompletedAt = at,
                DurationSeconds = seconds
            });

            if (daysAgo % 2 == 0)
            {
                int pre = 6 + daysAgo % 3;
                sessions.Add(new SessionResultDTO
                {
                    ItemKey = key,
                    CompletedAt = at,
                    DurationSeconds = seconds,
                    PreIntensity = pre,
                    PostIntensity = Math.Max(1, pre - 2 - daysAgo % 2)
                });
            }
        }

        return new AppBackupDTO
        {
            ExportedAtUtc = nowUtc,
            MoodEntries = moods,
            Completions = completions,
            SessionResults = sessions,
            ChatSessions = [PresentationChat(nowUtc), TensionChat(nowUtc.AddHours(-26))]
        };
    }

    /// <summary>A talk that ends on an offer of a practice: the first thing shown in the chat.</summary>
    private static BackupChatSessionDTO PresentationChat(DateTime nowUtc) => Talk(
        nowUtc.AddMinutes(-14),
        "Волнение перед презентацией",
        [
            Say("Завтра важная презентация, и я не могу перестать об этом думать"),
            Say("Боюсь, что всё пойдёт не так и меня осудят"),
            Tap(ChatQuickReplyKinds.Rating, "7"),
            Say("Сердце колотится и сложно сосредоточиться")
        ]);

    /// <summary>A talk that ends on the 0-10 question, which the screen shows as the tension slider.</summary>
    private static BackupChatSessionDTO TensionChat(DateTime nowUtc) => Talk(
        nowUtc,
        "Ссора с мамой",
        [
            Say("Сегодня очень тяжёлый день, всё валится из рук"),
            Say("Сильно поссорились с мамой, и теперь не могу успокоиться")
        ]);

    private abstract record Turn;

    private sealed record Text(string Value) : Turn;

    private sealed record QuickTap(string Kind, string Payload) : Turn;

    private static Turn Say(string text) => new Text(text);

    private static Turn Tap(string kind, string payload) => new QuickTap(kind, payload);

    private static BackupChatSessionDTO Talk(DateTime startUtc, string title, Turn[] turns)
    {
        CompanionDialogue dialogue = new(
            new LexiconSituationAnalyzer(),
            new KeywordCrisisDetector(),
            english: false,
            new Random(11),
            new FixedTime(startUtc),
            emotionGuesser: EmotionGuesser.Bundled);

        List<ChatMessageDTO> messages = [];
        DateTime at = startUtc;
        CompanionReply opening = dialogue.Open(new CompanionState(), previous: null);
        CompanionState state = opening.State;
        Add(messages, opening, ref at);

        foreach (Turn turn in turns)
        {
            at = at.AddSeconds(25);
            CompanionReply reply;
            if (turn is Text text)
            {
                messages.Add(new ChatMessageDTO { Role = ChatRole.User, Text = text.Value, CreatedAt = at });
                reply = dialogue.Respond(state, new CompanionInput.FreeText(text.Value));
            }
            else
            {
                QuickTap tap = (QuickTap)turn;
                messages.Add(new ChatMessageDTO { Role = ChatRole.User, Text = tap.Payload, CreatedAt = at });
                reply = dialogue.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(tap.Kind, tap.Payload, tap.Payload)));
            }

            state = reply.State;
            Add(messages, reply, ref at);
        }

        return new BackupChatSessionDTO
        {
            Session = new ChatSessionDTO
            {
                Title = title,
                CreatedAt = startUtc,
                UpdatedAt = at,
                Emotion = state.Emotion,
                Theme = state.Theme,
                FirstIntensity = state.FirstIntensity,
                LastIntensity = state.LastIntensity,
                StateJson = state.Serialize()
            },
            Messages = messages
        };
    }

    private static void Add(List<ChatMessageDTO> messages, CompanionReply reply, ref DateTime at)
    {
        for (int i = 0; i < reply.Messages.Count; i++)
        {
            at = at.AddSeconds(4);
            messages.Add(new ChatMessageDTO
            {
                Role = ChatRole.Companion,
                Text = reply.Messages[i],
                CreatedAt = at,
                QuickReplies = i == reply.Messages.Count - 1 ? reply.QuickReplies : []
            });
        }
    }
}
