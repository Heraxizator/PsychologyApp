using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.DependencyInjection;
using PsychologyApp.Bootstrap;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.DependencyInjection;
using PsychologyApp.Presentation.Infrastructure;

#if ANDROID
using PsychologyApp.Presentation.Platforms.Android.Renderers;
#endif

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
                fonts.AddFont("Roboto-Medium.ttf", "RobotoMedium");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("Roboto-SemiBold.ttf", "RobotoSemiBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler(typeof(Frame), typeof(ShadowFrameRenderer));
#endif
            });

        ConfigureHandlers();
        builder.AddPsychologyAppConfiguration();

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#else
        builder.Logging.SetMinimumLevel(LogLevel.Information);
#endif

        builder.Services.AddPsychologyAppCore(settings =>
        {
            settings.QuotApiUrl = string.IsNullOrWhiteSpace(settings.QuotApiUrl)
                ? Constants.QuotApiUrl
                : settings.QuotApiUrl;
            settings.ReviewEmailAddress = string.IsNullOrWhiteSpace(settings.ReviewEmailAddress)
                ? Constants.ReviewEmailAdress
                : settings.ReviewEmailAddress;
            settings.SmallTimeoutMs = settings.SmallTimeoutMs > 0 ? settings.SmallTimeoutMs : Constants.SmallBaseTimeout;
            settings.MiddleTimeoutMs = settings.MiddleTimeoutMs > 0 ? settings.MiddleTimeoutMs : Constants.MiddleBaseTimeout;
            settings.LargeTimeoutMs = settings.LargeTimeoutMs > 0 ? settings.LargeTimeoutMs : Constants.LargeBaseTimeout;
        });

        builder.Services.AddSingleton<MauiReasonContentProvider>();
        builder.Services.AddCachedReasonContentProvider<MauiReasonContentProvider>();
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
