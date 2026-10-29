using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Body;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Body;

public sealed class BodyCheckTests
{
    private sealed class MemoryRepository : IBodyRepository
    {
        public BodyLog? Log { get; set; }

        public Task<BodyLog?> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(Log);

        public Task SaveAsync(BodyLog log, CancellationToken cancellationToken = default)
        {
            Log = log;
            return Task.CompletedTask;
        }
    }

    private sealed class Clock(DateTimeOffset now) : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = now;

        public override DateTimeOffset GetUtcNow() => Now;
    }

    private static readonly DateTime Evening = new(2026, 10, 12, 20, 0, 0, DateTimeKind.Utc);

    private static BodyEntry Entry(string id, BodyArea area, BodySensation sensation, DateTime at, params BodyFeeling[] feelings) =>
        new(id, at, area, sensation, 5, feelings, null);

    [Fact]
    public void A_lump_in_the_throat_is_first_read_as_sadness_or_hurt()
    {
        IReadOnlyList<BodyFeeling> guesses = BodyRules.Guesses(BodyArea.Throat, BodySensation.Lump);

        Assert.Equal([BodyFeeling.Hurt, BodyFeeling.Sadness], guesses.Take(2).Order());
    }

    [Fact]
    public void Heat_in_the_face_is_read_as_shame_or_anger()
    {
        IReadOnlyList<BodyFeeling> guesses = BodyRules.Guesses(BodyArea.Face, BodySensation.Heat);

        Assert.Equal([BodyFeeling.Anger, BodyFeeling.Shame], guesses.Take(2).Order());
    }

    [Fact]
    public void Every_place_and_sensation_has_guesses_and_never_more_than_three()
    {
        foreach (BodyArea area in Enum.GetValues<BodyArea>())
        {
            foreach (BodySensation sensation in Enum.GetValues<BodySensation>())
            {
                IReadOnlyList<BodyFeeling> guesses = BodyRules.Guesses(area, sensation);
                Assert.InRange(guesses.Count, 1, BodyRules.MaxGuesses);
                Assert.DoesNotContain(BodyFeeling.Unknown, guesses);
            }
        }
    }

    [Theory]
    [InlineData(BodySensation.Pain, 2, true)]
    [InlineData(BodySensation.Tight, 8, true)]
    [InlineData(BodySensation.Tight, 7, false)]
    public void Pain_or_a_strong_sensation_sends_to_a_doctor_first(BodySensation sensation, int intensity, bool doctor) =>
        Assert.Equal(doctor, BodyRules.AsksForDoctor(sensation, intensity));

    [Fact]
    public void Practices_start_from_the_body_and_follow_the_feeling()
    {
        Assert.Equal([TechniqueId.BodyScan], BodyRules.Practices([]));
        Assert.Equal([TechniqueId.BodyScan, TechniqueId.Breathing, TechniqueId.Grounding], BodyRules.Practices([BodyFeeling.Fear]));
        Assert.Contains(TechniqueId.SelfCompassion, BodyRules.Practices([BodyFeeling.Shame]));
    }

    [Fact]
    public void A_pattern_needs_a_repeat_and_names_the_feeling_and_the_time_it_comes()
    {
        BodyEntry[] entries =
        [
            Entry("1", BodyArea.Throat, BodySensation.Lump, Evening, BodyFeeling.Hurt),
            Entry("2", BodyArea.Throat, BodySensation.Lump, Evening.AddDays(1), BodyFeeling.Hurt, BodyFeeling.Sadness),
            Entry("3", BodyArea.Throat, BodySensation.Lump, Evening.AddDays(2).AddHours(-12), BodyFeeling.Unknown),
            Entry("4", BodyArea.Chest, BodySensation.Tight, Evening, BodyFeeling.Anxiety)
        ];

        BodyPattern pattern = Assert.Single(BodyPatterns.Find(entries, TimeZoneInfo.Utc));

        Assert.Equal((BodyArea.Throat, BodySensation.Lump, 3), (pattern.Area, pattern.Sensation, pattern.Count));
        Assert.Equal(BodyFeeling.Hurt, pattern.Feeling);
        Assert.Equal(PartOfDay.Evening, pattern.MostlyAt);
    }

    [Fact]
    public void Check_ins_spread_over_the_day_have_no_usual_time_and_unknown_is_not_a_feeling()
    {
        BodyEntry[] entries =
        [
            Entry("1", BodyArea.Stomach, BodySensation.Tight, Evening, BodyFeeling.Unknown),
            Entry("2", BodyArea.Stomach, BodySensation.Tight, Evening.AddHours(-12))
        ];

        BodyPattern pattern = Assert.Single(BodyPatterns.Find(entries, TimeZoneInfo.Utc));

        Assert.Null(pattern.Feeling);
        Assert.Null(pattern.MostlyAt);
    }

    [Fact]
    public async Task A_check_in_is_saved_with_a_trimmed_note_and_a_clamped_intensity()
    {
        MemoryRepository repository = new();
        BodyService service = new(repository, new KeywordCrisisDetector(), new Clock(new DateTimeOffset(Evening)));

        BodyCheckResult result = await service.SaveAsync(BodyArea.Chest, BodySensation.Heavy, 14, [BodyFeeling.Sadness, BodyFeeling.Sadness], "  " + new string('a', 700));

        BodyEntry saved = Assert.Single(repository.Log!.Entries);
        Assert.Equal(result.Entry, saved);
        Assert.Equal(10, saved.Intensity);
        Assert.Equal([BodyFeeling.Sadness], saved.Feelings);
        Assert.Equal(BodyService.MaxNote, saved.Note!.Length);
        Assert.Equal(Evening, saved.CreatedUtc);
        Assert.False(result.Crisis);
    }

    [Fact]
    public async Task A_note_with_crisis_words_leads_to_help()
    {
        BodyService service = new(new MemoryRepository(), new KeywordCrisisDetector());

        BodyCheckResult result = await service.SaveAsync(BodyArea.Chest, BodySensation.Heavy, 6, [], "не хочу жить");

        Assert.True(result.Crisis);
    }

    [Fact]
    public async Task Crisis_words_after_the_storage_limit_still_lead_to_help()
    {
        BodyService service = new(new MemoryRepository(), new KeywordCrisisDetector());
        string note = new string('a', BodyService.MaxNote) + " не хочу жить";

        BodyCheckResult result = await service.SaveAsync(BodyArea.Chest, BodySensation.Heavy, 6, [], note);

        Assert.True(result.Crisis);
        Assert.Equal(BodyService.MaxNote, result.Entry.Note!.Length);
    }

    [Fact]
    public async Task The_log_keeps_the_latest_check_ins_and_one_can_be_deleted()
    {
        MemoryRepository repository = new()
        {
            Log = new BodyLog
            {
                Entries = Enumerable.Range(0, BodyService.MaxEntries)
                    .Select(i => Entry($"old{i}", BodyArea.Back, BodySensation.Tight, Evening.AddDays(-1000 + i)))
                    .ToList()
            }
        };
        BodyService service = new(repository, new KeywordCrisisDetector());

        BodyCheckResult result = await service.SaveAsync(BodyArea.Legs, BodySensation.Trembling, 3, [], null);

        Assert.Equal(BodyService.MaxEntries, repository.Log!.Entries.Count);
        Assert.DoesNotContain(repository.Log.Entries, entry => entry.Id == "old0");
        Assert.Equal(result.Entry.Id, repository.Log.Entries[^1].Id);

        await service.DeleteAsync(result.Entry.Id);
        Assert.DoesNotContain(repository.Log.Entries, entry => entry.Id == result.Entry.Id);
    }

    [Fact]
    public async Task A_scale_left_untouched_is_saved_as_not_rated()
    {
        MemoryRepository repository = new();
        BodyService service = new(repository, new KeywordCrisisDetector());

        await service.SaveAsync(BodyArea.Chest, BodySensation.Tight, null, [], null);

        Assert.Null(Assert.Single(repository.Log!.Entries).Intensity);
        Assert.False(BodyRules.AsksForDoctor(BodySensation.Tight, null));
    }

    [Fact]
    public void A_log_from_a_backup_file_is_made_safe_before_it_is_used()
    {
        BodyLog fromFile = new()
        {
            Entries =
            [
                new BodyEntry("ok", Evening, BodyArea.Chest, BodySensation.Tight, 42, null!, "  " + new string('n', 900)),
                new BodyEntry("", Evening, BodyArea.Chest, BodySensation.Tight, 3, [], null),
                new BodyEntry("dated", default, BodyArea.Chest, BodySensation.Tight, 3, [], null),
                null!
            ]
        };

        BodyEntry kept = Assert.Single(BodyService.Sanitize(fromFile).Entries);

        Assert.Equal("ok", kept.Id);
        Assert.Equal(10, kept.Intensity);
        Assert.Empty(kept.Feelings);
        Assert.Equal(BodyService.MaxNote, kept.Note!.Length);
    }

    [Fact]
    public void A_place_or_feeling_unknown_to_this_version_is_skipped_but_kept()
    {
        BodyLog log = System.Text.Json.JsonSerializer.Deserialize<BodyLog>("""
            {"Entries":[
              {"Id":"new","CreatedUtc":"2026-10-12T20:00:00Z","Area":"Knees","Sensation":"Lump","Intensity":3,"Feelings":[],"Note":null},
              {"Id":"a","CreatedUtc":"2026-10-12T20:00:00Z","Area":"Throat","Sensation":"Lump","Intensity":3,"Feelings":["Awe","Hurt"],"Note":null},
              {"Id":"b","CreatedUtc":"2026-10-13T20:00:00Z","Area":"Throat","Sensation":"Lump","Intensity":3,"Feelings":["Awe"],"Note":null}
            ]}
            """)!;

        Assert.Equal(3, log.Entries.Count);
        Assert.Equal(["a", "b"], log.Readable().Select(entry => entry.Id));
        BodyPattern pattern = Assert.Single(BodyPatterns.Find(log.Entries, TimeZoneInfo.Utc));
        Assert.Equal((BodyArea.Throat, 2, BodyFeeling.Hurt), (pattern.Area, pattern.Count, pattern.Feeling));
    }

    [Fact]
    public void Restoring_a_backup_twice_adds_its_check_ins_once_and_keeps_the_new_ones()
    {
        BodyLog local = new() { Entries = [Entry("new", BodyArea.Chest, BodySensation.Tight, Evening)] };
        BodyLog backup = new() { Entries = [Entry("old", BodyArea.Throat, BodySensation.Lump, Evening.AddDays(-3)), Entry("new", BodyArea.Head, BodySensation.Heat, Evening)] };

        BodyLog once = BodyService.Merge(local, backup);
        BodyLog twice = BodyService.Merge(once, backup);

        Assert.Equal(["old", "new"], twice.Entries.Select(entry => entry.Id));
        Assert.Equal(BodyArea.Chest, twice.Entries[1].Area);
    }
}
