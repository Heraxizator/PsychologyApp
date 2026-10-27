using System.Globalization;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Chat.ChatList;

/// <summary>
/// Turns a chat's colour group (<see cref="ChatCardLook"/>) into the tile colour (parameter "tile") or the icon colour ("icon"), as on
/// the test cards. A chat with no group keeps the accent tile. Follows the theme the list was built in; the list is rebuilt each time
/// it appears, which is also when a theme changed in the settings takes effect.
/// </summary>
public sealed class ChatTileColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        (Color tile, Color? icon) = FlavorTile.Colours(value is TechniqueFlavor flavor ? flavor : TechniqueFlavor.None);
        return parameter as string == "icon"
            ? icon ?? AppResources.Color("Primary", Colors.DodgerBlue)
            : tile;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
