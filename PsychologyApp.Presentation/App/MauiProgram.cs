using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.DependencyInjection;
using PsychologyApp.Application.Reason;
using PsychologyApp.Bootstrap;
using PsychologyApp.Domain.Base.Constants;
using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Abstractions;
using PsychologyApp.Presentation.Shared.Platform;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;

namespace PsychologyApp.Presentation.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement(isAndroidForegroundServiceEnabled: false)
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
        builder.Services.AddPsychologyAppCachedReasonContent(sp =>
            new CachedReasonContentProvider(
                sp.GetRequiredService<MauiReasonContentProvider>(),
                () => AppStrings.Language));
        builder.Services.AddSingleton<MauiQuotContentProvider>();
        builder.Services.AddPsychologyAppCachedQuotContent<MauiQuotContentProvider>();
        builder.Services.AddSingleton<MauiTestAssetReader>();
        builder.Services.AddSingleton<ITestAssetReader>(sp => sp.GetRequiredService<MauiTestAssetReader>());
        builder.Services.AddSingleton<TestCatalogService>();
        builder.Services.AddSingleton<CachedTestCatalogService>();
        builder.Services.AddSingleton<ITestCatalogService>(sp => sp.GetRequiredService<CachedTestCatalogService>());
        builder.Services.AddSingleton<LanguageContentReloader>();
        builder.Services.AddPsychologyAppPresentation();
        builder.Services.AddSingleton<AppShell>();

        MauiApp app = builder.Build();
        NavigationCoordinator.SetLogger(
            app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("NavigationCoordinator"));
        return app;
    }

    private static void ConfigureHandlers()
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("TransparentBackground", (handler, _) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.Background = null;
            handler.PlatformView.SetPadding(0, 0, 0, 0);
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
#endif
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("TransparentBackground", (handler, _) =>
        {
#if ANDROID
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.Background = null;
            handler.PlatformView.SetPadding(0, 0, 0, 0);
#elif IOS || MACCATALYST
            handler.PlatformView.Layer.BorderWidth = 0;
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
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
