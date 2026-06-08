using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.DependencyInjection;
using PsychologyApp.Bootstrap;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.DependencyInjection;
using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "InterRegular");
                fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
            });

        ConfigureHandlers();
        ReduceMotion.Configure(ReduceMotionDetector.IsEnabled);
        builder.AddPsychologyAppConfiguration();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        builder.Services.AddPsychologyAppCore(settings =>
        {
            settings.ReviewEmailAddress = string.IsNullOrWhiteSpace(settings.ReviewEmailAddress)
                ? Constants.ReviewEmailAdress
                : settings.ReviewEmailAddress;
            settings.SmallTimeoutMs = settings.SmallTimeoutMs > 0 ? settings.SmallTimeoutMs : Constants.SmallBaseTimeout;
            settings.MiddleTimeoutMs = settings.MiddleTimeoutMs > 0 ? settings.MiddleTimeoutMs : Constants.MiddleBaseTimeout;
            settings.LargeTimeoutMs = settings.LargeTimeoutMs > 0 ? settings.LargeTimeoutMs : Constants.LargeBaseTimeout;
        });

        builder.Services.AddSingleton<MauiReasonContentProvider>();
        builder.Services.AddCachedReasonContentProvider<MauiReasonContentProvider>();
        builder.Services.AddSingleton<MauiQuotContentProvider>();
        builder.Services.AddCachedQuotContentProvider<MauiQuotContentProvider>();
        builder.Services.AddSingleton<ContentCacheInvalidator>();
        builder.Services.AddPsychologyAppPresentation();
        builder.Services.AddSingleton<AppShell>();

        MauiApp app = builder.Build();
        MauiServiceProvider.Current = app.Services;
        return app;
    }

    private static void ConfigureHandlers()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("TransparentBackground", (handler, _) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("TransparentBackground", (handler, _) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
        });

        Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("TransparentBackground", (handler, _) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
        });

    }
}
