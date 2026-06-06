namespace PsychologyApp.Presentation.Infrastructure;

/// <summary>
/// Static access to DI during MAUI startup before <c>MauiContext</c> is available.
/// Allowed callers only: <c>AppShell</c>, <c>GlobalExceptionHandler</c>.
/// ViewModels and pages must use constructor injection — do not call this type elsewhere.
/// </summary>
public static class MauiServiceProvider
{
    public static IServiceProvider Current { get; internal set; } = default!;

    public static IServiceProvider Required
    {
        get
        {
            if (Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services is IServiceProvider services)
            {
                return services;
            }

            return Current
                ?? throw new InvalidOperationException("Application services are not available yet.");
        }
    }

    public static T GetRequiredService<T>() where T : notnull =>
        Required.GetRequiredService<T>();

    public static T GetRequired<T>() where T : notnull => GetRequiredService<T>();
}
