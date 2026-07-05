using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherColorPassRowView : ContentView
{
    public LuscherColorPassRowView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty PassTitleProperty =
        BindableProperty.Create(nameof(PassTitle), typeof(string), typeof(LuscherColorPassRowView), string.Empty);

    public static readonly BindableProperty ColorsProperty =
        BindableProperty.Create(
            nameof(Colors),
            typeof(IReadOnlyList<LuscherColorDisplayItem>),
            typeof(LuscherColorPassRowView),
            Array.Empty<LuscherColorDisplayItem>(),
            propertyChanged: OnColorsChanged);

    public string PassTitle
    {
        get => (string)GetValue(PassTitleProperty);
        set => SetValue(PassTitleProperty, value);
    }

    public IReadOnlyList<LuscherColorDisplayItem> Colors
    {
        get => (IReadOnlyList<LuscherColorDisplayItem>)GetValue(ColorsProperty);
        set => SetValue(ColorsProperty, value);
    }

    private static void OnColorsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LuscherColorPassRowView view)
        {
            view.RebuildPassRow();
        }
    }

    private void RebuildPassRow()
    {
        PassRow.Children.Clear();
        if (Colors.Count == 0)
        {
            return;
        }

        double swatchSize = (double)Microsoft.Maui.Controls.Application.Current!.Resources["LuscherSwatchSizeSmall"];

        for (int index = 0; index < Colors.Count; index++)
        {
            LuscherColorDisplayItem item = Colors[index];
            PassRow.Children.Add(new LuscherColorSwatchView
            {
                SwatchColor = item.MauiColor,
                ColorName = item.Name,
                SwatchSize = swatchSize,
                ShowName = false
            });

            if (index < Colors.Count - 1)
            {
                PassRow.Children.Add(new Label
                {
                    Text = "→",
                    Style = (Style)Microsoft.Maui.Controls.Application.Current.Resources["CaptionStyle"],
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Microsoft.Maui.Controls.Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Microsoft.Maui.Controls.Application.Current.Resources["TextSecondaryDark"]
                        : (Color)Microsoft.Maui.Controls.Application.Current.Resources["TextTertiaryLight"]
                });
            }
        }
    }
}
