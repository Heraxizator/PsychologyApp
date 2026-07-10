using MauiIcons.Material;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// Canonical Material icon name strings used by EmptyState and other UI.
/// Values must parse as <see cref="MaterialIcons"/>.
/// </summary>
public static class MaterialIconNames
{
    public const string Search = nameof(MaterialIcons.Search);
    public const string SearchOff = nameof(MaterialIcons.SearchOff);
    public const string SelfImprovement = nameof(MaterialIcons.SelfImprovement);
    public const string Assignment = nameof(MaterialIcons.Assignment);
    public const string History = nameof(MaterialIcons.History);
    public const string FavoriteBorder = nameof(MaterialIcons.FavoriteBorder);
    public const string FormatQuote = nameof(MaterialIcons.FormatQuote);
    public const string DoneAll = nameof(MaterialIcons.DoneAll);
    public const string AutoAwesome = nameof(MaterialIcons.AutoAwesome);
    public const string CheckCircle = nameof(MaterialIcons.CheckCircle);
    public const string Whatshot = nameof(MaterialIcons.Whatshot);
    public const string EmojiEvents = nameof(MaterialIcons.EmojiEvents);
    public const string WorkspacePremium = nameof(MaterialIcons.WorkspacePremium);
    public const string Favorite = nameof(MaterialIcons.Favorite);
    public const string Insights = nameof(MaterialIcons.Insights);
    public const string LibraryMusic = nameof(MaterialIcons.LibraryMusic);

    public static bool TryResolve(string? iconName, out MaterialIcons icon)
    {
        if (string.IsNullOrWhiteSpace(iconName))
        {
            icon = default;
            return false;
        }

        return Enum.TryParse(iconName, ignoreCase: false, out icon);
    }
}
