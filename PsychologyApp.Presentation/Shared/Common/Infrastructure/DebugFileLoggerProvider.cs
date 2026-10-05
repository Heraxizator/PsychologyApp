using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PsychologyApp.Presentation.Shared.Common.Infrastructure;

/// <summary>
/// Writes log entries to a local file: everything in Debug builds, warnings and errors in Release (where it is the only log there is).
/// Lines are queued and written by one background task, so logging never touches the disk on the caller's
/// (often the UI) thread.
/// </summary>
public sealed class DebugFileLoggerProvider : ILoggerProvider
{
    private const long MaxFileBytes = 2 * 1024 * 1024;

    private readonly string _filePath;
    private readonly LogLevel _minimumLevel;
    private readonly Channel<string> _lines = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions { SingleReader = true });

    /// <summary>Where Release builds keep warnings and errors, for the "share the error log" action.</summary>
    public static string ErrorLogPath => Path.Combine(FileSystem.AppDataDirectory, "logs", "app-errors.log");

    private static readonly object SyncWriteLock = new();

    /// <summary>Writes straight to disk (no queue): for a crash, where the process may be gone before the queue is drained.</summary>
    public static void AppendNow(string filePath, string text)
    {
        try
        {
            lock (SyncWriteLock)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                File.AppendAllText(filePath, text + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Nothing more can be done while crashing.
        }
    }

    public DebugFileLoggerProvider(string filePath, LogLevel minimumLevel = LogLevel.Trace)
    {
        _filePath = filePath;
        _minimumLevel = minimumLevel;
        _ = Task.Run(WriteLoopAsync);
    }

    public ILogger CreateLogger(string categoryName) =>
        new DebugFileLogger(categoryName, _lines.Writer, _minimumLevel);

    public void Dispose() => _lines.Writer.TryComplete();

    private async Task WriteLoopAsync()
    {
        try
        {
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Start fresh when the previous runs have grown the file past the cap.
            if (File.Exists(_filePath) && new FileInfo(_filePath).Length > MaxFileBytes)
            {
                File.Delete(_filePath);
            }

            await using StreamWriter writer = new(_filePath, append: true, Encoding.UTF8);
            ChannelReader<string> reader = _lines.Reader;
            while (await reader.WaitToReadAsync().ConfigureAwait(false))
            {
                while (reader.TryRead(out string? line))
                {
                    await writer.WriteLineAsync(line).ConfigureAwait(false);
                }

                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            // Never fail the app because of log file I/O.
        }
    }

    private sealed class DebugFileLogger(string categoryName, ChannelWriter<string> lines, LogLevel minimumLevel) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && logLevel >= minimumLevel;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string line =
                $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}] {logLevel} {categoryName}: {formatter(state, exception)}";

            if (exception is not null)
            {
                line += Environment.NewLine + exception;
            }

            lines.TryWrite(line);
        }
    }
}
