using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

public class OpenRouterProvider : IAIPsychologistProvider, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string ModelName = "deepseek/deepseek-r1"; // Убрали ":free"
    private const string ApiUrl = "https://openrouter.ai/api/v1/chat/completions";

    public bool IsInitialized { get; private set; }
    public string ProviderName => "OpenRouter.ai (DeepSeek R1)";

    public OpenRouterProvider(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(2)
        };

        // Заголовки согласно документации OpenRouter.ai
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/your-repo");
        _httpClient.DefaultRequestHeaders.Add("X-Title", "Psychology App");
    }

    public async Task<bool> InitializeAsync()
    {
        try
        {
            var requestBody = new
            {
                model = ModelName,
                messages = new[]
                {
                    new { role = "user", content = "Hello" }
                },
                max_tokens = 10
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                IsInitialized = true;
                return true;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                IsInitialized = false;
                return false;
            }

            IsInitialized = true;
            return true;
        }
        catch (Exception)
        {
            IsInitialized = false;
            return false;
        }
    }

    public async Task<string> GenerateResponseAsync(string userMessage)
    {
        if (!IsInitialized)
        {
            return "OpenRouter API не инициализирован. Проверьте API ключ.";
        }

        try
        {
            var systemMessage = "Ты - профессиональный психолог, который помогает людям решать их проблемы. " +
                              "Отвечай кратко, поддерживающе и конструктивно. " +
                              "Используй эмпатию и давай практические советы. " +
                              "Отвечай на русском языке.";

            var requestBody = new
            {
                model = ModelName,
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return "Ошибка авторизации. Проверьте правильность API ключа OpenRouter.";
                }

                try
                {
                    var errorResponse = JsonSerializer.Deserialize<OpenRouterErrorResponse>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return $"Ошибка API: {errorResponse?.Error?.Message ?? responseContent}";
                }
                catch
                {
                    return $"Ошибка API: {response.StatusCode}. {responseContent}";
                }
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent, jsonOptions);

            if (result?.Choices != null && result.Choices.Length > 0)
            {
                var generatedText = result.Choices[0].Message?.Content?.Trim();
                if (!string.IsNullOrWhiteSpace(generatedText))
                {
                    return generatedText;
                }
            }

            return "Не удалось получить ответ от OpenRouter API.";
        }
        catch (HttpRequestException ex)
        {
            return $"Ошибка подключения: {ex.Message}";
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return "Ошибка: Превышено время ожидания ответа от API.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }

    private class OpenRouterResponse
    {
        [JsonPropertyName("choices")]
        public OpenRouterChoice[]? Choices { get; set; }
    }

    private class OpenRouterChoice
    {
        [JsonPropertyName("message")]
        public OpenRouterMessage? Message { get; set; }
    }

    private class OpenRouterMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private class OpenRouterErrorResponse
    {
        [JsonPropertyName("error")]
        public OpenRouterError? Error { get; set; }
    }

    private class OpenRouterError
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("code")]
        public int? Code { get; set; }
    }
}
