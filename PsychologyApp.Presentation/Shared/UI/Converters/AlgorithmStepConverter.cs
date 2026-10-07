using System.Globalization;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Shared.UI.Converters;

/// <summary>Reads one stored step ("2. Inhale") for the step badge: parameter "number" gives the digit, "text" the words, "badge" whether there is a number to show.</summary>
public sealed class AlgorithmStepConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        AlgorithmStep step = AlgorithmStep.Parse(value as string);
        string? kind = parameter as string;
        return kind switch
        {
            "number" => step.Number?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            "badge" => step.Number is not null,
            _ => step.Text
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
