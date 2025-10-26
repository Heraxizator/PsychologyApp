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
    public static async Task<Quot> GetSingleQuotFromApi(CancellationToken cancellationToken)
    {
        string? jsonResult = await HttpExtention.GetAsync(Constants.QuotApiUrl, cancellationToken) 
            ?? throw new QuotApiLoadException("Не удалось получить данные от Quots API"); ;

        QuotAPI? quotAPI = JsonSerializer.Deserialize<QuotAPI>(jsonResult) ??
            throw new QuotApiLoadException("Не удалось получить данные от Quots API"); ;

        return Quot.Create(quotAPI.QuoteAuthor!, quotAPI.QuoteText!, quotAPI.QuoteAuthor!, false, false);
    }
}
