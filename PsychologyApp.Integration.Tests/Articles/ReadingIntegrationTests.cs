using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Articles;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Articles;

/// <summary>Reading progress through the real services and SQLite: a course answer is kept, and it travels with a backup.</summary>
public sealed class ReadingIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _sourceFactory = new();
    private readonly SharedMemoryConnectionFactory _targetFactory = new();
    private ServiceProvider _source = null!;
    private ServiceProvider _target = null!;

    public Task InitializeAsync()
    {
        _source = IntegrationTestServiceCollection.BuildCoreProvider(_sourceFactory);
        _target = IntegrationTestServiceCollection.BuildCoreProvider(_targetFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task A_course_day_and_saved_articles_are_stored_and_restored_from_a_backup()
    {
        IReadingService source = _source.GetRequiredService<IReadingService>();
        await source.StartCourseAsync(CourseIds.Sleep);
        Assert.Equal(CourseDayOutcome.Done, await source.CompleteDayAsync(CourseIds.Sleep, 1, "I go to bed at 11"));
        await source.ToggleSavedAsync(ArticleIds.Panic);

        ReadingState stored = await source.GetAsync();
        Assert.Equal("I go to bed at 11", Assert.Single(stored.Course(CourseIds.Sleep)!.Days).Reflection);

        string json = await _source.GetRequiredService<IBackupService>().ExportAsync();
        await _target.GetRequiredService<IBackupService>().ImportAsync(json);

        ReadingState restored = await _target.GetRequiredService<IReadingService>().GetAsync();
        Assert.Equal([ArticleIds.Panic], restored.Saved);
        Assert.Equal("I go to bed at 11", Assert.Single(restored.Course(CourseIds.Sleep)!.Days).Reflection);
    }

    public async Task DisposeAsync()
    {
        await _sourceFactory.DisposeAsync();
        await _targetFactory.DisposeAsync();
        await _source.DisposeAsync();
        await _target.DisposeAsync();
    }
}
