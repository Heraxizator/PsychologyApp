// The view models are plain C# apart from a handful of MAUI types. These stand-ins let the real view-model files be compiled into
// the net10.0 test project, so their commands and state are tested without a device or the MAUI runtime.
using System.Windows.Input;

namespace Microsoft.Maui.Controls
{
    public interface INavigation;

    public sealed class Command<T> : ICommand
    {
        private readonly Action<T> _execute;

        public Command(Action<T> execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute(parameter is T value ? value : default!);
    }

    public sealed class Command : ICommand
    {
        private readonly Action _execute;

        public Command(Action execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}

namespace Microsoft.Maui.ApplicationModel
{
    public static class MainThread
    {
        public static bool IsMainThread => true;

        public static void BeginInvokeOnMainThread(Action action) => action();

        public static Task InvokeOnMainThreadAsync(Action action)
        {
            action();
            return Task.CompletedTask;
        }
    }

    public sealed class Launcher
    {
        public static Launcher Default { get; } = new();

        public static List<string> Opened { get; } = [];

        public static Exception? FailWith { get; set; }

        public Task<bool> OpenAsync(string uri)
        {
            if (FailWith is not null)
            {
                throw FailWith;
            }

            Opened.Add(uri);
            return Task.FromResult(true);
        }
    }

    public enum BrowserLaunchMode
    {
        SystemPreferred
    }

    public sealed class Browser
    {
        public static Browser Default { get; } = new();

        public static List<string> Opened { get; } = [];

        public Task<bool> OpenAsync(string uri, BrowserLaunchMode mode)
        {
            Opened.Add(uri);
            return Task.FromResult(true);
        }
    }
}

namespace PsychologyApp.Presentation.Shared.Common
{
    /// <summary>Only the defaults the linked <c>UserPreferencesState</c> reads; the real class talks to the MAUI Preferences API.</summary>
    public static class UserPreferences
    {
        public const string DefaultLanguage = "ru";
        public const string DefaultTheme = "light";
        public const string DefaultColor = "blue";
        public const string DefaultForm = "rounded";
        public const string DefaultSize = "medium";
        public const int DefaultPracticeReminderHour = 19;
        public const int DefaultQuoteReminderHour = 9;
        public const int DefaultMoodReminderHour = 20;
        public const int DefaultChatReminderHour = 20;

        public static bool IsEnglish(string language) => language.Equals("en", StringComparison.OrdinalIgnoreCase);
    }
}

namespace PsychologyApp.Presentation.Shared.Services.Preferences
{
    using PsychologyApp.Domain.Practice;
    using PsychologyApp.Presentation.Shared.Common;

    /// <summary>In-memory stand-in for the store that wraps the MAUI Preferences API.</summary>
    public sealed class MauiUserPreferencesStore : IUserPreferencesStore
    {
        private UserPreferencesState _state = new();

        public event Action? Changed;

        public UserPreferencesState Load() => _state;

        public void Save(UserPreferencesState state)
        {
            _state = state;
            Changed?.Invoke();
        }

        public void ApplyAll()
        {
        }

        public void ApplyPreview(UserPreferencesState state)
        {
        }

        public void CompleteOnboarding(string concern, bool? practiceRemindersEnabled = null, int? practiceReminderHour = null)
        {
        }

        public void ResetOnboardingCompletion()
        {
        }

        public void SetPendingTechnique(TechniqueId techniqueId)
        {
        }

        public TechniqueId? ConsumePendingTechnique() => null;

        public bool HasUsedPhysicsSearch => false;

        public void MarkPhysicsSearchUsed()
        {
        }

        public void SetPendingOpenJournal()
        {
        }

        public bool ConsumePendingOpenJournal() => false;

        public string PersistedLanguage => _state.Language;

        public void SetPendingQuoteFeed(string feedKey)
        {
        }

        public string? ConsumePendingQuoteFeed() => null;
    }
}

namespace PsychologyApp.Presentation.Entities.Test
{
    // The navigation interface imports this namespace; its types now live in Application.
    internal static class Anchor;
}

namespace PsychologyApp.Presentation.Shared.Navigation
{
    internal static class Anchor;
}

namespace PsychologyApp.Presentation.Shared.Common.Infrastructure
{
    public static class DebugFileLoggerProvider
    {
        public static string ErrorLogPath { get; set; } = Path.Combine(Path.GetTempPath(), "app-errors-test.log");
    }
}

namespace PsychologyApp.Presentation.Shared.Common
{
    /// <summary>The real one hops to the MAUI main thread; in tests everything already runs inline.</summary>
    public static class UiThread
    {
        public static Task RunAsync(Action action)
        {
            action();
            return Task.CompletedTask;
        }

        public static Task RunAsync(Func<Task> action) => action();
    }
}

namespace Microsoft.Maui.ApplicationModel.Communication
{
    public sealed class PhoneDialer
    {
        public static PhoneDialer Default { get; } = new();

        public static List<string> Dialled { get; } = [];

        public bool IsSupported => false;

        public void Open(string number) => Dialled.Add(number);
    }
}

namespace Microsoft.Maui.Storage
{
    public static class FileSystem
    {
        public static string CacheDirectory { get; set; } = Path.GetTempPath();
    }

    public sealed class FileResult(string fullPath)
    {
        public string FullPath { get; } = fullPath;
    }

    public sealed class PickOptions
    {
        public string? PickerTitle { get; set; }
    }

    public sealed class FilePicker
    {
        public static FilePicker Default { get; } = new();

        /// <summary>What the next call to <see cref="PickAsync"/> returns (null means the person cancelled).</summary>
        public static FileResult? Next { get; set; }

        public Task<FileResult?> PickAsync(PickOptions? options = null) => Task.FromResult(Next);
    }
}

namespace Microsoft.Maui.ApplicationModel.DataTransfer
{
    public sealed class ShareFile(string path)
    {
        public string FullPath { get; } = path;
    }

    public sealed class ShareFileRequest
    {
        public string? Title { get; set; }

        public ShareFile? File { get; set; }
    }

    public sealed class Share
    {
        public static Share Default { get; } = new();

        /// <summary>The content of every file that was shared, read while the file still existed.</summary>
        public static List<(string Title, string FileName, string Content)> Shared { get; } = [];

        public Task RequestAsync(ShareFileRequest request)
        {
            Shared.Add((request.Title ?? string.Empty, Path.GetFileName(request.File!.FullPath), System.IO.File.ReadAllText(request.File.FullPath)));
            return Task.CompletedTask;
        }
    }
}

namespace Microsoft.Maui
{
    public readonly record struct Thickness(double Left, double Top, double Right, double Bottom);
}

namespace Microsoft.Maui.Controls
{
    public readonly record struct LayoutOptions(int Value)
    {
        public static LayoutOptions Start { get; } = new(0);

        public static LayoutOptions End { get; } = new(1);
    }
}
