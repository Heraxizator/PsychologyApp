namespace PsychologyApp.Application.Configuration;

public sealed class AppSettings
{
    public string QuotApiUrl { get; set; } = "https://api.forismatic.com/api/1.0/?method=getQuote&format=json";
    public string ReviewEmailAddress { get; set; } = string.Empty;
    public int SmallTimeoutMs { get; set; } = 5000;
    public int MiddleTimeoutMs { get; set; } = 10000;
    public int LargeTimeoutMs { get; set; } = 15000;
    public int DbCommandTimeoutSeconds { get; set; } = 30;
    public string ReviewSmsRecipient { get; set; } = string.Empty;
}
