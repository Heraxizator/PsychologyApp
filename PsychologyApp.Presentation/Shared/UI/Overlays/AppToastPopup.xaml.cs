using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Devices;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Overlays;

public partial class AppToastPopup : Popup
{
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public AppToastPopup(string message, AppToastKind kind = AppToastKind.Info)
    {
        InitializeComponent();
        MessageLabel.Text = message;
        ApplyKind(kind);
        Loaded += OnLoaded;
    }

    public Task WaitUntilReadyAsync() => _ready.Task;

    public void PrepareEnterAnimation() => UiAnimations.PrepareForToastPop(ToastCardHost);

    public Task AnimateInAsync() => UiAnimations.AnimateToastPopInAsync(ToastCardHost);

    public Task AnimateOutAsync() => UiAnimations.AnimateToastPopOutAsync(ToastCardHost);

    public async Task DismissAsync()
    {
        await AnimateOutAsync();
        await CloseAsync();
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        ApplyHostSize();
        PrepareEnterAnimation();
        await AnimateInAsync();
        _ready.TrySetResult();
    }

    private void ApplyHostSize()
    {
        DisplayInfo display = DeviceDisplay.MainDisplayInfo;
        ToastHost.HeightRequest = display.Height / display.Density;
        ToastHost.WidthRequest = display.Width / display.Density;
    }

    private void ApplyKind(AppToastKind kind)
    {
        ResourceDictionary resources = Microsoft.Maui.Controls.Application.Current!.Resources;

        (string iconName, Color accentColor) = kind switch
        {
            AppToastKind.Success => ("CheckCircle", (Color)resources["Primary"]),
            AppToastKind.Error => ("ErrorOutline", (Color)resources["Yellow100Accent"]),
            _ => ("Info", (Color)resources["Primary"]),
        };

        ToastIcon.IconName = iconName;
        ToastIcon.IconColor = accentColor;
        AccentBar.BackgroundColor = accentColor;
    }
}
