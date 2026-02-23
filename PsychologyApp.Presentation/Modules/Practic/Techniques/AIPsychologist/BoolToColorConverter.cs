using System.Globalization;
using Microsoft.Maui.Controls;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isUser)
        {
            // Если это сообщение пользователя - светло-голубой фон
            // Если это сообщение AI - белый фон
            return isUser ? Color.FromArgb("#E3F2FD") : Colors.White;
        }
        return Colors.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
