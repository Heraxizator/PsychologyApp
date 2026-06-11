using PsychologyApp.Domain.Entities;
using PsychologyApp.Testing.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Quots;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public class QuotRepositoryTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private readonly QuotRepository _repository;

    public QuotRepositoryTests()
    {
        _repository = new QuotRepository(_connectionFactory, RepositoryTestContext.Settings);
    }

    [Fact]
    public async Task GetUnreadLatestAsync_ReturnsOnlyUnreadQuotes()
    {
        await _repository.AddAsync(Quot.Create("A", "Unread quote", "general", isReaded: false, isFavourite: false));
        Quot read = Quot.Create("B", "Read quote", "general", isReaded: true, isFavourite: false);
        await _repository.AddAsync(read);

        var unread = (await _repository.GetUnreadLatestAsync(10)).ToList();

        Assert.All(unread, quote => Assert.False(quote.IsReaded));
        Assert.Contains(unread, quote => quote.Text == "Unread quote");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();
}
