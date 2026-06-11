namespace PsychologyApp.Presentation.Common;

public static class SearchTextHighlighter
{
    public static FormattedString Build(string? text, string query)
    {
        var formatted = new FormattedString();
        if (string.IsNullOrEmpty(text))
        {
            return formatted;
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            formatted.Spans.Add(new Span { Text = text });
            return formatted;
        }

        int startIndex = 0;
        while (startIndex < text.Length)
        {
            int matchIndex = text.IndexOf(query, startIndex, StringComparison.OrdinalIgnoreCase);
            if (matchIndex < 0)
            {
                formatted.Spans.Add(new Span { Text = text[startIndex..] });
                break;
            }

            if (matchIndex > startIndex)
            {
                formatted.Spans.Add(new Span { Text = text[startIndex..matchIndex] });
            }

            formatted.Spans.Add(new Span
            {
                Text = text.Substring(matchIndex, query.Length),
                TextColor = Microsoft.Maui.Controls.Application.Current?.Resources["Primary"] as Color,
                FontAttributes = FontAttributes.Bold
            });

            startIndex = matchIndex + query.Length;
        }

        return formatted;
    }
}
