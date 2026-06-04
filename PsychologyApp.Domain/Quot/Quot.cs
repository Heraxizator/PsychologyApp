using PsychologyApp.Domain.Base;

namespace PsychologyApp.Domain.Entities;

public class Quot : Entity
{
    public long QuotId { get; private init; }
    public string Title { get; private set; } = default!;
    public string Text { get; private set; } = default!;
    public string Theme { get; private set; } = default!;
    public bool IsReaded { get; private set; }
    public bool IsFavourite { get; private set; }

    public static Quot Create(string title, string text, string theme, bool isReaded, bool isFavourite)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        return new Quot
        {
            Title = title,
            Text = text,
            Theme = theme,
            IsReaded = isReaded,
            IsFavourite = isFavourite
        };
    }

    public void EditTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title;
    }

    public void EditText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Text = text;
    }

    public void MarkAsReaded() => IsReaded = true;

    public void SetFavourite(bool isFavourite) => IsFavourite = isFavourite;
}
