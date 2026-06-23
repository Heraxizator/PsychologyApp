namespace PsychologyApp.Presentation.Features.ManageQuotes;

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
