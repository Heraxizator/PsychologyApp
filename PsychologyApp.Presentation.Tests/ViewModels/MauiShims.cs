// The view models are plain C# apart from a handful of MAUI types. These stand-ins let the real view-model files be compiled into
// the net10.0 test project, so their commands and state are tested without a device or the MAUI runtime.
using System.Windows.Input;

namespace Microsoft.Maui.Controls
{
    public class Page;

    public class ContentPage : Page;

    /// <summary>The MAUI navigation contract, member for member, so a test can stand in for the page stack.</summary>
    public interface INavigation
    {
        IReadOnlyList<Page> NavigationStack { get; }

        IReadOnlyList<Page> ModalStack { get; }

        void InsertPageBefore(Page page, Page before);

        Task<Page> PopAsync();

        Task<Page> PopAsync(bool animated);

        Task<Page> PopModalAsync();

        Task<Page> PopModalAsync(bool animated);

        Task PopToRootAsync();

        Task PopToRootAsync(bool animated);

        Task PushAsync(Page page);

        Task PushAsync(Page page, bool animated);

        Task PushModalAsync(Page page);

        Task PushModalAsync(Page page, bool animated);

        void RemovePage(Page page);
    }

    public interface IValueConverter
    {
        object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture);

        object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture);
    }

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
        private readonly Action<object> _execute;

        public Command(Action execute) => _execute = _ => execute();

        public Command(Action<object> execute) => _execute = execute;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute(parameter!);
    }
}

namespace Microsoft.Maui.ApplicationModel
{
    public sealed class FeatureNotSupportedException : Exception;

    public sealed class PermissionException : Exception;

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

    public sealed class EmailMessage
    {
        public string? Subject { get; set; }

        public string? Body { get; set; }

        public List<string>? To { get; set; }
    }

    public sealed class Email
    {
        public static Email Default { get; } = new();

        public static List<EmailMessage> Composed { get; } = [];

        public Task ComposeAsync(EmailMessage message)
        {
            Composed.Add(message);
            return Task.CompletedTask;
        }
    }

    public sealed class SmsMessage(string body, IEnumerable<string> recipients)
    {
        public string Body { get; } = body;

        public List<string> Recipients { get; } = [.. recipients];
    }

    public sealed class Sms
    {
        public static Sms Default { get; } = new();

        public static List<SmsMessage> Composed { get; } = [];

        public Task ComposeAsync(SmsMessage message)
        {
            Composed.Add(message);
            return Task.CompletedTask;
        }
    }
}

namespace Microsoft.Maui.Storage
{
    /// <summary>Key-value settings kept in memory for the test run.</summary>
    public sealed class Preferences
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _values = new(StringComparer.Ordinal);

        public static Preferences Default { get; } = new();

        public string Get(string key, string defaultValue) => _values.TryGetValue(key, out string? value) ? value : defaultValue;

        public void Set(string key, string value) => _values[key] = value;
    }

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

    public sealed class ShareTextRequest
    {
        public string? Title { get; set; }

        public string? Text { get; set; }
    }

    public sealed class Share
    {
        public static Share Default { get; } = new();

        /// <summary>The content of every file that was shared, read while the file still existed.</summary>
        public static List<(string Title, string FileName, string Content)> Shared { get; } = [];

        /// <summary>The text of every text share.</summary>
        public static List<string> SharedTexts { get; } = [];

        public static Exception? FileShareError { get; set; }

        public Task RequestAsync(ShareTextRequest request)
        {
            SharedTexts.Add(request.Text ?? string.Empty);
            return Task.CompletedTask;
        }

        public Task RequestAsync(ShareFileRequest request)
        {
            if (FileShareError is { } error)
            {
                return Task.FromException(error);
            }
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

namespace PsychologyApp.Presentation.Shared.Common
{
    /// <summary>The persistence half of the real class (it reads the MAUI Preferences API) reduced to what the linked logic half needs.</summary>
    public static partial class UserPreferences
    {
        public static string Persisted { get; set; } = DefaultLanguage;

        private static UserPreferencesState Current => new() { Language = Persisted };

        public static string GetPersistedLanguage() => Persisted;
    }
}

namespace MauiIcons.Material
{
    /// <summary>The icons the app names; the real enum comes from the MauiIcons package.</summary>
    public enum MaterialIcons
    {
        Assignment, AutoAwesome, CheckCircle, DoneAll, EmojiEvents, Favorite, FavoriteBorder, FormatQuote, History, Insights, LibraryMusic, Search, SearchOff, SelfImprovement, Whatshot, WorkspacePremium
    }
}

namespace Microsoft.Maui.Graphics
{
    /// <summary>Only what the view models do with a colour: make it from the hex code of a test result.</summary>
    public sealed record Color(string Hex)
    {
        public static Color FromArgb(string hex) => new(hex);
    }

    public static class Colors
    {
        public static Color Transparent { get; } = new("#00000000");
        public static Color Black { get; } = new("#000000");
        public static Color Gray { get; } = new("#808080");
        public static Color Red { get; } = new("#FF0000");
        public static Color Green { get; } = new("#008000");
        public static Color Blue { get; } = new("#0000FF");
        public static Color Yellow { get; } = new("#FFFF00");
        public static Color Purple { get; } = new("#800080");
        public static Color Brown { get; } = new("#A52A2A");
    }
}

namespace Microsoft.Maui.Dispatching
{
    public interface IDispatcher
    {
        bool Dispatch(Action action);
    }
}

namespace Microsoft.Maui.Devices
{
    public enum HapticFeedbackType
    {
        Click,
        LongPress
    }

    public sealed class HapticFeedback
    {
        public static HapticFeedback Default { get; } = new();

        public bool IsSupported => false;

        public void Perform(HapticFeedbackType type)
        {
        }
    }
}
