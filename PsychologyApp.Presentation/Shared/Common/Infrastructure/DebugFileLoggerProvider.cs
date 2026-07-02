using Microsoft.Extensions.Logging;

namespace PsychologyApp.Presentation.Shared.Common.Infrastructure;

/// <summary>
/// Writes log entries to a local file (Debug builds). Complements <c>AddDebug()</c> for post-mortem inspection.
/// </summary>
public sealed class DebugFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public DebugFileLoggerProvider(string filePath) =>
        _filePath = filePath;

    public ILogger CreateLogger(string categoryName) =>
        new DebugFileLogger(categoryName, _filePath);

    public void Dispose()
    {
    }

    private sealed class DebugFileLogger(string categoryName, string filePath) : ILogger
    {
        private static readonly object Gate = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

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

            string message = formatter(state, exception);
            string line =
                $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff}] {logLevel} {categoryName}: {message}";

            if (exception is not null)
            {
                line += Environment.NewLine + exception;
            }

            lock (Gate)
            {
                try
                {
                    string? directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
                catch
                {
                    // Never fail app startup because of log file I/O.
                }
            }
        }
    }
}
