namespace PsychologyApp.Presentation.Services.Quotes;

public interface IQuotesChangeNotifier
{
    event Action? FavoritesChanged;

    void NotifyFavoritesChanged();
}

public sealed class QuotesChangeNotifier : IQuotesChangeNotifier
{
    public event Action? FavoritesChanged;

    public void NotifyFavoritesChanged() => FavoritesChanged?.Invoke();
}
