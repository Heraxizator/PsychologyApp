using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Devices;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Shared.UI.Overlays;

public partial class AppOptionsSheetPopup : Popup
{
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public AppOptionsSheetPopup(string title, IReadOnlyList<string> options, string cancelLabel)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        CancelLabel.Text = cancelLabel;

        foreach (string option in options)
        {
            AddOptionRow(option);
        }

        Loaded += OnLoaded;
        Closed += (_, _) => _closed.TrySetResult();
    }

    public string? SelectedOption { get; private set; }

    private void OnLoaded(object? sender, EventArgs e)
    {
        ApplyHostSize();
        UiAnimations.AnimateToastPopInAsync(SheetCardHost, slideOffset: 12).FireAndForget();
    }

    private void ApplyHostSize()
    {
        // Use the MAUI window (usable area), not raw display pixels — full display
        // height pushes the bottom Auto row under the system gesture/nav inset.
        if (GetUsableWindowSize() is (double width, double height))
        {
            SheetHost.WidthRequest = width;
            SheetHost.HeightRequest = height;
            return;
        }

        DisplayInfo display = DeviceDisplay.MainDisplayInfo;
        SheetHost.WidthRequest = display.Width / display.Density;
        SheetHost.HeightRequest = display.Height / display.Density;
    }

    private static (double Width, double Height)? GetUsableWindowSize()
    {
        Window? window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window is null || window.Width <= 0 || window.Height <= 0)
        {
            return null;
        }

        return (window.Width, window.Height);
    }

    private void AddOptionRow(string option)
    {
        var row = new Border
        {
            BackgroundColor = Colors.Transparent,
            Stroke = Colors.Transparent,
            MinimumHeightRequest = 48,
            Padding = new Thickness(0, 4),
        };

        var label = new Label
        {
            Text = option,
            Style = (Style)Microsoft.Maui.Controls.Application.Current!.Resources["AppDialogActionLabelStyle"],
            InputTransparent = true,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        string captured = option;
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => SelectOption(captured);
        row.GestureRecognizers.Add(tap);
        row.Content = label;
        OptionsStack.Add(row);
    }

    private async void SelectOption(string option)
    {
        SelectedOption = option;
        await CloseAsync();
    }

    private async void OnCancelTapped(object? sender, TappedEventArgs e) => await CloseAsync();

    private async void OnDismissTapped(object? sender, TappedEventArgs e) => await CloseAsync();
}
