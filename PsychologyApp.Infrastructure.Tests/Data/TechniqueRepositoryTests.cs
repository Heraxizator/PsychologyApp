using PsychologyApp.Application.Exceptions;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.Data.Repositories.Techniques;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public class TechniqueRepositoryTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private readonly TechniqueRepository _repository;

    public TechniqueRepositoryTests()
    {
        _repository = new TechniqueRepository(_connectionFactory, RepositoryTestContext.Settings);
    }

    [Fact]
    public async Task AddAsync_GetByIdAsync_ReturnsPersistedTechnique()
    {
        var technique = Technique.Create(0, "1", "2026-06-05", "Header", "Description", "Subject", "Author", "Step 1", "img.png");

        long id = await _repository.AddAsync(technique);
        Technique? loaded = await _repository.GetByIdAsync(id);

        Assert.NotNull(loaded);
        Assert.Equal("Header", loaded!.Header);
        Assert.Equal("Author", loaded.Author);
    }

    [Fact]
    public async Task GetLatestAsync_ReturnsMostRecentFirst()
    {
        await _repository.AddAsync(Technique.Create(0, "1", "2026-06-05", "First", "d", "s", "a", "alg", "img"));
        await _repository.AddAsync(Technique.Create(0, "2", "2026-06-05", "Second", "d", "s", "a", "alg", "img"));

        var latest = (await _repository.GetLatestAsync(1)).ToList();

        Assert.Single(latest);
        Assert.Equal("Second", latest[0].Header);
    }

    [Fact]
    public async Task EditAsync_UpdatesTechnique()
    {
        long id = await _repository.AddAsync(Technique.Create(0, "1", "2026-06-05", "Old", "d", "s", "a", "alg", "img"));
        Technique technique = (await _repository.GetByIdAsync(id))!;
        technique.SetHeader("New");

        bool updated = await _repository.EditAsync(technique);
        Technique? loaded = await _repository.GetByIdAsync(id);

        Assert.True(updated);
        Assert.Equal("New", loaded!.Header);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTechnique()
    {
        long id = await _repository.AddAsync(Technique.Create(0, "1", "2026-06-05", "DeleteMe", "d", "s", "a", "alg", "img"));
        Technique technique = (await _repository.GetByIdAsync(id))!;

        bool deleted = await _repository.DeleteAsync(technique);
        Technique? loaded = await _repository.GetByIdAsync(id);

        Assert.True(deleted);
        Assert.Null(loaded);
    }

    [Fact]
    public async Task EditAsync_WhenEntityMissing_ThrowsPersistenceException()
    {
        var missing = Technique.Create(999, "1", "2026-06-05", "Missing", "d", "s", "a", "alg", "img");

        await Assert.ThrowsAsync<PersistenceException>(() => _repository.EditAsync(missing));
    }

    [Fact]
    public async Task AddAsync_ReturnsPositiveId()
    {
        var technique = Technique.Create(0, "1", "2026-06-05", "Header", "Description", "Subject", "Author", "Step 1", "img.png");

        long id = await _repository.AddAsync(technique);

        Assert.True(id > 0);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();
}
