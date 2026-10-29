using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Articles;

public sealed class CourseTests
{
    private sealed class MemoryRepository : IReadingRepository
    {
        public ReadingState? State { get; private set; }

        public Task<ReadingState?> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(State);

        public Task SaveAsync(ReadingState state, CancellationToken cancellationToken = default)
        {
            State = state;
            return Task.CompletedTask;
        }
    }

    private sealed class Clock(DateTimeOffset now) : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = now;

        public override DateTimeOffset GetUtcNow() => Now;

        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
    }

    private static readonly DateTimeOffset Monday = new(2026, 10, 12, 9, 0, 0, TimeSpan.Zero);

    private static (ReadingService Service, MemoryRepository Repository, Clock Clock) Create()
    {
        MemoryRepository repository = new();
        Clock clock = new(Monday);
        return (new ReadingService(repository, new KeywordCrisisDetector(), clock), repository, clock);
    }

    private static readonly string[] AllLanguages = ["ru", "en", "fr", "de", "es", "pt", "zh"];

    public static TheoryData<string> Languages => new(AllLanguages);

    [Theory]
    [MemberData(nameof(Languages))]
    public void Every_language_has_the_same_courses_as_english(string language)
    {
        IReadOnlyList<Course> ru = ArticleContent.Courses(language);
        IReadOnlyList<Course> en = ArticleContent.Courses("en");

        Assert.Equal(ru.Select(course => (course.Id, course.TestId, course.Icon, course.Days.Count)), en.Select(course => (course.Id, course.TestId, course.Icon, course.Days.Count)));
        foreach ((CourseDay r, CourseDay e) in ru.SelectMany(course => course.Days).Zip(en.SelectMany(course => course.Days)))
        {
            Assert.Equal(
                r.Blocks.Select(block => (block.Kind, block.Technique, block.ArticleId, block.TestId, block.Items?.Count, block.Threshold, block.Correct)),
                e.Blocks.Select(block => (block.Kind, block.Technique, block.ArticleId, block.TestId, block.Items?.Count, block.Threshold, block.Correct)));
            Assert.Equal(r.Minutes, e.Minutes);
        }
    }

    [Fact]
    public void Every_day_has_reading_a_task_and_a_question()
    {
        foreach (CourseDay day in AllLanguages.SelectMany(ArticleContent.Courses).SelectMany(course => course.Days))
        {
            Assert.False(string.IsNullOrWhiteSpace(day.Title));
            Assert.Contains(day.Blocks, block => block.Kind == ArticleBlockKind.Paragraph);
            Assert.False(string.IsNullOrWhiteSpace(day.Task));
            Assert.False(string.IsNullOrWhiteSpace(day.Reflection));
        }
    }

    [Theory]
    [MemberData(nameof(Languages))]
    public async Task Course_links_name_practices_and_articles_as_they_are_called(string language)
    {
        TechniqueCatalogService catalog = new(new BuiltInTechniqueCatalogProvider(() => language));

        foreach (ArticleBlock block in ArticleContent.Courses(language).SelectMany(course => course.Days).SelectMany(day => day.Blocks))
        {
            if (block.Kind == ArticleBlockKind.Practice)
            {
                BuiltInTechniqueDefinition definition = await catalog.GetAsync(block.Technique!.Value);
                Assert.Equal(definition.ListTitle, block.Text);
            }
            else if (block.Kind == ArticleBlockKind.Article)
            {
                Assert.Equal(ArticleContent.Find(block.ArticleId!, language)?.Title, block.Text);
            }
        }
    }

    [Fact]
    public void Courses_that_speak_of_self_harm_or_danger_lead_to_crisis_help()
    {
        // The mood course names thoughts of death, the trauma course an ongoing danger: both must give the way to help on day one.
        foreach (string language in AllLanguages)
        {
            foreach (string id in new[] { CourseIds.Mood, CourseIds.Trauma })
            {
                Assert.Contains(ArticleContent.FindCourse(id, language)!.Days[0].Blocks, block => block.Kind == ArticleBlockKind.CrisisHelp);
            }
        }
    }

    [Fact]
    public async Task The_next_day_opens_on_the_next_calendar_day()
    {
        // Each day's task needs a day to be tried: closing day 1 must not open day 2 the same evening.
        (ReadingService service, _, Clock clock) = Create();
        await service.StartCourseAsync(CourseIds.Sleep);

        Assert.Equal(CourseDayOutcome.Done, await service.CompleteDayAsync(CourseIds.Sleep, 1, "Ложусь в 23, встаю в 7"));
        CourseProgress progress = (await service.GetAsync()).Course(CourseIds.Sleep)!;
        Assert.Equal(CourseDayState.Done, service.StateOf(1, progress));
        Assert.Equal(CourseDayState.Tomorrow, service.StateOf(2, progress));
        Assert.Equal(CourseDayState.Locked, service.StateOf(3, progress));
        Assert.Equal(CourseDayOutcome.NotOpen, await service.CompleteDayAsync(CourseIds.Sleep, 2, null));

        clock.Now = Monday.AddDays(1);
        Assert.Equal(CourseDayState.Open, service.StateOf(2, progress));
        Assert.Equal(CourseDayOutcome.Done, await service.CompleteDayAsync(CourseIds.Sleep, 2, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(ReadingService.MaxReflection)]
    public async Task An_answer_that_reads_as_a_crisis_does_not_close_the_day(int prefixLength)
    {
        (ReadingService service, MemoryRepository repository, _) = Create();
        await service.StartCourseAsync(CourseIds.Mood);

        CourseDayOutcome outcome = await service.CompleteDayAsync(CourseIds.Mood, 1, new string('a', prefixLength) + " не хочу жить");

        Assert.Equal(CourseDayOutcome.Crisis, outcome);
        Assert.Empty(repository.State!.Course(CourseIds.Mood)!.Days);
    }

    [Fact]
    public async Task The_last_day_finishes_the_course()
    {
        (ReadingService service, _, Clock clock) = Create();
        int days = ArticleContent.FindCourse(CourseIds.Trauma, "ru")!.Days.Count;
        for (int day = 1; day <= days; day++)
        {
            Assert.Equal(CourseDayOutcome.Done, await service.CompleteDayAsync(CourseIds.Trauma, day, null));
            clock.Now = clock.Now.AddDays(1);
        }

        Assert.NotNull((await service.GetAsync()).Course(CourseIds.Trauma)!.FinishedUtc);
    }

    [Fact]
    public async Task Saving_twice_unsaves_and_reading_is_remembered_once()
    {
        (ReadingService service, MemoryRepository repository, _) = Create();

        Assert.True(await service.ToggleSavedAsync(ArticleIds.Panic));
        Assert.False(await service.ToggleSavedAsync(ArticleIds.Panic));
        await service.MarkReadAsync(ArticleIds.Panic);
        await service.MarkReadAsync(ArticleIds.Panic);

        Assert.Empty(repository.State!.Saved);
        Assert.Equal([ArticleIds.Panic], repository.State.Read);
    }

    private static readonly DateTime Now = Monday.UtcDateTime;

    private static TestResultDTO Result(string testId, int score, int daysAgo = 1) => new() { TestId = testId, Score = score, CompletedAt = Now.AddDays(-daysAgo) };

    [Fact]
    public void A_high_score_recommends_the_matching_course()
    {
        IReadOnlyList<ReadingPick> picks = ReadingRecommender.Pick([Result("isi", 18), Result("gad7", 6)], [], new ReadingState(), Now);

        Assert.Equal(new ReadingPick(CourseIds.Sleep, true, ReadingPickReason.Test, "isi"), picks[0]);
        Assert.Equal(new ReadingPick(ArticleIds.Anxiety, false, ReadingPickReason.Test, "gad7"), picks[1]);
    }

    [Fact]
    public void Trauma_comes_before_the_courses_for_its_symptoms()
    {
        // Poor sleep and low mood after trauma are its signs; the sleep course alone would miss the cause.
        IReadOnlyList<ReadingPick> picks = ReadingRecommender.Pick([Result("isi", 20), Result("pcl5", 40), Result("phq9", 14)], [], new ReadingState(), Now);

        Assert.Equal(CourseIds.Trauma, picks[0].Id);
    }

    [Fact]
    public void Old_results_and_finished_courses_are_not_recommended()
    {
        ReadingState finished = new() { Courses = [new CourseProgress(CourseIds.Anxiety, Now.AddDays(-30), [], Now.AddDays(-20))] };

        Assert.Empty(ReadingRecommender.Pick([Result("gad7", 15)], [], finished, Now));
        Assert.Empty(ReadingRecommender.Pick([Result("isi", 20, daysAgo: 120)], [], new ReadingState(), Now));
    }

    [Fact]
    public void Several_hard_days_in_the_journal_suggest_the_mood_course()
    {
        MoodEntryDTO[] moods = [.. Enumerable.Range(1, 3).Select(day => new MoodEntryDTO { MoodLevel = 1, RecordedAt = Now.AddDays(-day) })];

        Assert.Equal(new ReadingPick(CourseIds.Mood, true, ReadingPickReason.Mood), Assert.Single(ReadingRecommender.Pick([], moods, new ReadingState(), Now)));
    }

    [Fact]
    public void A_course_in_progress_and_an_unfinished_article_come_first()
    {
        ReadingState state = new()
        {
            LastArticleId = ArticleIds.Ocd,
            LastOpenedUtc = Now.AddDays(-1),
            Courses = [new CourseProgress(CourseIds.Sleep, Now.AddDays(-2), [new CourseDayRecord(1, Now.AddDays(-2), null)])]
        };

        IReadOnlyList<ReadingPick> picks = ReadingRecommender.Pick([Result("gad7", 12)], [], state, Now);

        Assert.Equal(
            [new ReadingPick(CourseIds.Sleep, true, ReadingPickReason.ContinueCourse), new ReadingPick(ArticleIds.Ocd, false, ReadingPickReason.ContinueReading), new ReadingPick(CourseIds.Anxiety, true, ReadingPickReason.Test, "gad7")],
            picks);
    }

    [Fact]
    public void The_measure_compares_the_start_with_the_latest_result()
    {
        // The person must see the course from where it started: a result taken long before it, or the same result twice, says nothing.
        DateTime started = Now.AddDays(-10);
        TestResultDTO[] history = [Result("isi", 21, daysAgo: 200), Result("isi", 18, daysAgo: 12), Result("isi", 9, daysAgo: 1)];

        Assert.Equal(new CourseMeasurement(18, 9), CourseMeasure.Of(history, started));
        Assert.Equal(new CourseMeasurement(null, 9), CourseMeasure.Of(history, null));
        Assert.Equal(new CourseMeasurement(null, 21), CourseMeasure.Of([Result("isi", 21, daysAgo: 200)], started));
        Assert.Equal(new CourseMeasurement(15, 9), CourseMeasure.Of([Result("isi", 15, daysAgo: 9), Result("isi", 9, daysAgo: 1)], started));
    }
}
