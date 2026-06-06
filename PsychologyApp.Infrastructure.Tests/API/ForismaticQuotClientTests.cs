using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.API.Quots;
using System.Net;
using System.Text;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.API;

public class ForismaticQuotClientTests
{
    [Fact]
    public async Task FetchRandomQuotAsync_ParsesSuccessfulResponse()
    {
        const string json = """{"quoteText":"Test quote","quoteAuthor":"Author"}""";
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var client = new ForismaticQuotClient(
            new StubHttpClientFactory(handler),
            Options.Create(new AppSettings { QuotApiUrl = "https://example.test/quote" }));

        var quot = await client.FetchRandomQuotAsync();

        Assert.Equal("Test quote", quot.Text);
        Assert.Equal("Author", quot.Title);
        Assert.Equal("general", quot.Theme);
    }

    [Fact]
    public async Task FetchRandomQuotAsync_ThrowsWhenResponseExceedsSizeLimit()
    {
        string oversizedJson = $$"""{"quoteText":"{{new string('x', 65 * 1024)}}","quoteAuthor":"Author"}""";
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(oversizedJson, Encoding.UTF8, "application/json")
        });

        var client = new ForismaticQuotClient(
            new StubHttpClientFactory(handler),
            Options.Create(new AppSettings { QuotApiUrl = "https://example.test/quote" }));

        await Assert.ThrowsAsync<PsychologyApp.Application.Exceptions.QuotApiLoadException>(
            () => client.FetchRandomQuotAsync());
    }

    [Fact]
    public async Task FetchRandomQuotAsync_ThrowsWhenPayloadMissing()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        });

        var client = new ForismaticQuotClient(
            new StubHttpClientFactory(handler),
            Options.Create(new AppSettings { QuotApiUrl = "https://example.test/quote" }));

        await Assert.ThrowsAsync<PsychologyApp.Application.Exceptions.QuotApiLoadException>(
            () => client.FetchRandomQuotAsync());
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(responder(request));
    }
}
