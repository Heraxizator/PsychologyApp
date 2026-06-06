using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PsychologyApp.Infrastructure.API.Quots;

public sealed class ForismaticQuotClient(IHttpClientFactory httpClientFactory, IOptions<AppSettings> settings) : IQuotApiClient
{
    private const int MaxResponseBytes = 64 * 1024;
    private const string DefaultQuoteTheme = "general";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Quot> FetchRandomQuotAsync(CancellationToken cancellationToken = default)
    {
        HttpClient client = httpClientFactory.CreateClient(nameof(ForismaticQuotClient));
        string requestUrl = settings.Value.QuotApiUrl;

        using HttpResponseMessage response = await SendWithRetryAsync(client, requestUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new QuotApiLoadException($"Quot API returned {(int)response.StatusCode}");
        }

        await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        byte[] payloadBytes = await ReadLimitedBytesAsync(stream, MaxResponseBytes, cancellationToken);

        ForismaticResponse? payload = JsonSerializer.Deserialize<ForismaticResponse>(payloadBytes, JsonOptions);
        if (payload is null || string.IsNullOrWhiteSpace(payload.QuoteText))
        {
            throw new QuotApiLoadException("Не удалось получить данные от Quots API");
        }

        string author = payload.QuoteAuthor ?? "Unknown";
        return Quot.Create(author, payload.QuoteText, DefaultQuoteTheme, isReaded: false, isFavourite: false);
    }

    private static async Task<byte[]> ReadLimitedBytesAsync(Stream stream, int maxBytes, CancellationToken cancellationToken)
    {
        using var buffer = new MemoryStream();
        byte[] chunk = new byte[4096];
        int totalRead = 0;

        while (true)
        {
            int read = await stream.ReadAsync(chunk.AsMemory(0, chunk.Length), cancellationToken);
            if (read == 0)
            {
                break;
            }

            totalRead += read;
            if (totalRead > maxBytes)
            {
                throw new QuotApiLoadException($"Quot API response exceeded {maxBytes} bytes");
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        return buffer.ToArray();
    }

    private static async Task<HttpResponseMessage> SendWithRetryAsync(HttpClient client, string requestUrl, CancellationToken cancellationToken)
    {
        Exception? lastError = null;
        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl, cancellationToken);
                if ((int)response.StatusCode >= 500 && attempt < 2)
                {
                    response.Dispose();
                    await Task.Delay(TimeSpan.FromMilliseconds(250 * (attempt + 1)), cancellationToken);
                    continue;
                }

                return response;
            }
            catch (Exception ex) when (attempt < 2 && ex is not OperationCanceledException)
            {
                lastError = ex;
                await Task.Delay(TimeSpan.FromMilliseconds(250 * (attempt + 1)), cancellationToken);
            }
        }

        throw lastError is null
            ? new QuotApiLoadException("Не удалось выполнить запрос к Quots API")
            : new QuotApiLoadException("Не удалось выполнить запрос к Quots API", lastError);
    }

    private sealed class ForismaticResponse
    {
        [JsonPropertyName("quoteText")]
        public string? QuoteText { get; set; }

        [JsonPropertyName("quoteAuthor")]
        public string? QuoteAuthor { get; set; }
    }
}
