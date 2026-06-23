namespace PsychologyApp.Presentation.Shared.Common;

public static class QuoteShareFormatter
{
    public static string Format(string text, string author)
    {
        string safeAuthor = string.IsNullOrWhiteSpace(author) ? AppStrings.UnknownAuthor : author;
        return $"\"{text}\"\n\n— {safeAuthor}\n\n✦ {AppStrings.QuoteShareFooter}";
    }
}
