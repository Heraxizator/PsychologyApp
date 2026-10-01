using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PsychologyApp.Presentation.Shared.Common.Infrastructure;

/// <summary>
/// Writes log entries to a local file (Debug builds). Complements <c>AddDebug()</c> for post-mortem inspection.
/// Lines are queued and written by one background task, so logging never touches the disk on the caller's
/// (often the UI) thread.
/// </summary>
public sealed class DebugFileLoggerProvider : ILoggerProvider
{
    private const long MaxFileBytes = 2 * 1024 * 1024;

    private readonly string _filePath;
    private readonly Channel<string> _lines = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions { SingleReader = true });

    public DebugFileLoggerProvider(string filePath)
    {
        _filePath = filePath;
        _ = Task.Run(WriteLoopAsync);
    }

    public ILogger CreateLogger(string categoryName) =>
        new DebugFileLogger(categoryName, _lines.Writer);

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

    private sealed class DebugFileLogger(string categoryName, ChannelWriter<string> lines) : ILogger
    {
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
