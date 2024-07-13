using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Models;
using PsychologyApp.Domain.Common;
using PsychologyApp.Domain.Constants;
using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PsychologyApp.Application.ApiHandlers;

public static class QuotsHandler
{
    public static async Task<QuotDTO> GetQuotsFromApi(int cancelTimeout = 3000)
    {
        CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.CancelAfter(cancelTimeout);

        HttpClient httpClient = new();

        HttpResponseMessage response = await httpClient.GetAsync(Constants.QuotApiUrl, cancellationTokenSource.Token);

        if (response.IsSuccessStatusCode is false)
        {
            throw new QuotApiLoadException("Не удалось получить данные от Quots API");
        }

        string jsonResult = await response.Content.ReadAsStringAsync();

        QuotItem? quotItem = JsonSerializer.Deserialize<QuotItem>(jsonResult);

        QuotDTO quotDTO = new()
        {
            Text = quotItem?.QuoteText,
            Title = quotItem?.QuoteAuthor,
            IsReaded = false,
        };

        return quotDTO;
    }

    #region Objects

    internal class QuotItem
    {
        [JsonPropertyName("quoteText")]
        public string? QuoteText { get; set; }

        [JsonPropertyName("quoteAuthor")]
        public string? QuoteAuthor { get; set; }

        [JsonPropertyName("senderName")]
        public string? SenderName { get; set; }

        [JsonPropertyName("senderLink")]
        public string? SenderLink { get; set; }

        [JsonPropertyName("quoteLink")]
        public string? QuoteLink { get; set; }
    }

    #endregion
}
