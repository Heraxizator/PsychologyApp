using PsychologyApp.Domain.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PsychologyApp.Infrastructure.API.Base;

public static class HttpExtention
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task<string> GetAsync(string requestURI, int cancelTimeout = 15000)
    {
        using CancellationTokenSource cancellationTokenSource = new(cancelTimeout);

        HttpResponseMessage response = await _httpClient.GetAsync(requestURI, cancellationTokenSource.Token);

        if (response.IsSuccessStatusCode is false)
        {
            return string.Empty;
        }

        return await response.Content.ReadAsStringAsync();
    }
}
