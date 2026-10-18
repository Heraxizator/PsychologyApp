using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.Models;

namespace PsychologyApp.StoreAssets;

/// <summary>
/// A believable month of use as a backup file, in Russian or English: moods with short notes, a daily practice for the last week, two chats whose
/// companion lines come from the real dialogue engine (so the screenshots show what the app really says), the tension before and after practices
/// and the stress test taken twice. Imported into a debug build on the phone, it fills the screens without typing anything. Everything is invented;
/// nothing comes from a real person.
/// </summary>
public static class DemoBackup
{
    private static DateTime ToUtc(this DateTime phoneTime, TimeZoneInfo zone) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(phoneTime, DateTimeKind.Unspecified), zone);

    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    // Mood level 1..5 for the last 30 days, oldest first: a hard stretch that slowly eases, with ordinary dips.
    private static readonly int[] Levels = [2, 2, 3, 2, 3, 3, 2, 3, 4, 3, 3, 2, 3, 4, 4, 3, 4, 4, 3, 4, 5, 4, 4, 3, 4, 4, 5, 4, 4, 5];

    // Days ago -> note. The second line of some notes is a journal factor (the prefix the journal's factor chips write), so "What helps me"
    // has something to find: walks and sleep go with better days, work with worse ones.
    private static readonly Dictionary<int, (string Ru, string En)> Notes = new()
    {
        [29] = ("Плохо спалось, на работе всё валилось из рук\nРабота: дедлайн", "Slept badly, everything went wrong at work\nWork: deadline"),
        [27] = ("Поссорились с мамой, до сих пор обидно\nСтресс: ссора", "Argued with mum, it still hurts\nStress: an argument"),
        [24] = ("Прогулка вечером немного отвлекла\nПрогулка: в парке", "An evening walk took my mind off it\nWalk: in the park"),
        [21] = ("Весь день тревожно перед отчётом\nРабота: отчёт", "Anxious all day before the report\nWork: the report"),
        [19] = ("Опять допоздна за ноутбуком\nРабота: переработка", "At the laptop until late again\nWork: overtime"),
        [18] = ("Разговор с подругой помог, стало легче\nЛюди: подруга", "Talking to a friend helped, it got easier\nPeople: a friend"),
        [15] = ("Выспаться удалось, спокойный день\nСон: 8 часов", "Managed to sleep well, a calm day\nSleep: 8 hours"),
        [12] = ("Дыхательная практика помогла успокоиться", "The breathing practice helped me calm down"),
        [10] = ("Долгая прогулка после работы\nПрогулка: час пешком", "A long walk after work\nWalk: an hour on foot"),
        [9] = ("Усталость, но вечером получилась зарядка\nСпорт: зарядка", "Tired, but I managed some exercise in the evening\nSport: a short workout"),
        [7] = ("Нервный созвон с заказчиком\nРабота: созвон", "A tense call with a client\nWork: a call"),
        [6] = ("Хороший разговор с коллегой на работе\nЛюди: коллега", "A good talk with a colleague\nPeople: a colleague"),
        [4] = ("Прогулка в парке подняла настроение\nПрогулка: в парке", "A walk in the park lifted my mood\nWalk: in the park"),
        [3] = ("Легла пораньше, проснулась бодрой\nСон: рано легла", "Went to bed early, woke up rested\nSleep: an early night"),
        [2] = ("Спокойный день, получилось отдохнуть\nОтдых: выходной", "A quiet day, I got some rest\nRest: a day off"),
        [0] = ("Сегодня заметно легче\nПрогулка: утром", "Noticeably easier today\nWalk: in the morning")
    };

    private static readonly (string Key, string Ru, string En)[] Practices =
    [
        ("Breathing", "Квадратное дыхание", "Box breathing"),
        ("Grounding", "Заземление 5-4-3-2-1", "5-4-3-2-1 grounding"),
        ("Observer", "Позиция наблюдателя", "The observer"),
        ("ThoughtRecord", "Запись мысли", "Thought record"),
        ("SmallStep", "Один маленький шаг", "One small step"),
        ("SelfCompassion", "Добрые слова себе", "Kind words to yourself"),
        ("Anchor", "Якорь ресурса", "Resource anchor")
    ];

    /// <param name="zone">The phone's time zone: the days and hours are the phone's, not those of the computer that writes the file.</param>
    public static AppBackupDTO Build(DateTime nowUtc, bool english, TimeZoneInfo zone)
    {
        // The last day of the month of use: early in the morning that is yesterday, so no entry (the latest is at 21:25) lies in the future.
        DateTime today = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, zone).AddHours(-22).Date;
        List<MoodEntryDTO> moods = [];
        for (int i = 0; i < Levels.Length; i++)
        {
            int daysAgo = Levels.Length - 1 - i;
            DateTime day = today.AddDays(-daysAgo);
            moods.Add(new MoodEntryDTO
            {
                MoodLevel = Levels[i],
                Note = Notes.TryGetValue(daysAgo, out (string Ru, string En) note) ? (english ? note.En : note.Ru) : null,
                RecordedAt = day.AddHours(9).AddMinutes(10 + i % 7 * 3).ToUtc(zone)
            });

            if (i % 3 == 1)
            {
                moods.Add(new MoodEntryDTO { MoodLevel = Math.Clamp(Levels[i] + (i % 2 == 0 ? 1 : -1), 1, 5), RecordedAt = day.AddHours(21).AddMinutes(5).ToUtc(zone) });
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

            (string key, string ru, string en) = Practices[(29 - daysAgo) % Practices.Length];
            DateTime at = today.AddDays(-daysAgo).AddHours(daysAgo % 3 == 0 ? 19 : 8).AddMinutes(20 + daysAgo % 5 * 4).ToUtc(zone);
            int seconds = 180 + daysAgo % 4 * 60;
            completions.Add(new CompletionDTO
            {
                CompletionKind = "technique",
                ItemKey = key,
                ModuleName = english ? "Practices" : "Практик",
                PageName = english ? en : ru,
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
            TestResults = StressTests(today, english, zone),
            ChatSessions = [PresentationChat(today.AddHours(20).ToUtc(zone), english), TensionChat(today.AddDays(-1).AddHours(19).ToUtc(zone), english)]
        };
    }

    /// <summary>
    /// The stress test taken twice, a month and two weeks ago, with less stress the second time: "What helps me" shows the change and offers to
    /// take it again. The summaries are the app's own band texts.
    /// </summary>
    private static IReadOnlyList<TestResultDTO> StressTests(DateTime today, bool english, TimeZoneInfo zone) =>
    [
        new TestResultDTO
        {
            TestId = "pss10",
            Score = 24,
            Summary = english ? "14–26 — moderate stress" : "14–26 — умеренный стресс",
            CompletedAt = today.AddDays(-29).AddHours(20).ToUtc(zone)
        },
        new TestResultDTO
        {
            TestId = "pss10",
            Score = 12,
            Summary = english ? "0–13 — low stress" : "0–13 — низкий стресс",
            CompletedAt = today.AddDays(-15).AddHours(20).ToUtc(zone)
        }
    ];

    /// <summary>A talk that ends on an offer of a practice: the first thing shown in the chat.</summary>
    private static BackupChatSessionDTO PresentationChat(DateTime startUtc, bool english) => Talk(
        startUtc,
        english ? "Before the presentation" : "Перед презентацией",
        english,
        english
            ?
            [
                Say("I have an important presentation tomorrow and I can't stop thinking about it"),
                Say("I'm afraid it will all go wrong and people will judge me"),
                Tap(ChatQuickReplyKinds.Rating, "7"),
                Say("My heart is pounding and it's hard to focus")
            ]
            :
            [
                Say("Завтра важная презентация, и я не могу перестать об этом думать"),
                Say("Боюсь, что всё пойдёт не так и меня осудят"),
                Tap(ChatQuickReplyKinds.Rating, "7"),
                Say("Сердце колотится и сложно сосредоточиться")
            ]);

    /// <summary>A talk that ends on the 0-10 question, which the screen shows as the tension slider.</summary>
    private static BackupChatSessionDTO TensionChat(DateTime startUtc, bool english) => Talk(
        startUtc,
        english ? "An argument with mum" : "Ссора с мамой",
        english,
        english
            ?
            [
                Say("Today is a really hard day, everything is falling apart"),
                Say("I had a big argument with my mum and now I can't calm down")
            ]
            :
            [
                Say("Сегодня очень тяжёлый день, всё валится из рук"),
                Say("Сильно поссорились с мамой, и теперь не могу успокоиться")
            ]);

    private abstract record Turn;

    private sealed record Text(string Value) : Turn;

    private sealed record QuickTap(string Kind, string Payload) : Turn;

    private static Turn Say(string text) => new Text(text);

    private static Turn Tap(string kind, string payload) => new QuickTap(kind, payload);

    private static BackupChatSessionDTO Talk(DateTime startUtc, string title, bool english, Turn[] turns)
    {
        CompanionDialogue dialogue = new(
            new LexiconSituationAnalyzer(),
            new KeywordCrisisDetector(),
            english,
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
