using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Infrastructure.API.Base;
using PsychologyApp.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.API.Quots;

public static class QuotHandler
{
    public static async Task<bool> GetQuotsFromApi(int cancelTimeout = 15000)
    {
        CancellationTokenSource cancellationTokenSource = new(cancelTimeout);

        HttpClient httpClient = new();

        HttpResponseMessage response = await httpClient.GetAsync(Constants.QuotApiUrl, cancellationTokenSource.Token);

        if (response.IsSuccessStatusCode is false)
        {
            throw new QuotApiLoadException("Не удалось получить данные от Quots API");
        }

        string jsonResult = await HttpExtention.GetAsync(Constants.QuotApiUrl, cancelTimeout);

        QuotAPI? quotAPI = JsonSerializer.Deserialize<QuotAPI>(jsonResult);

        if (quotAPI is null)
        {
            return false;
        }

        long quotId = await Database.QuotRepository.AddAsync(Quot.Create(quotAPI.QuoteAuthor!, quotAPI.QuoteText!, quotAPI.QuoteAuthor, false));

        return quotId > 0;
    }
}
