namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Lets one tap through and swallows the ones that follow within <see cref="Interval"/> (a double tap).
/// For handlers that finish instantly (a wizard step, a toggle), where re-entrancy cannot be told from a second tap:
/// two quick taps on "Next" would otherwise skip a step.
/// </summary>
public sealed class TapDebouncer
{
    public static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(400);

    private readonly Func<long> _clockMilliseconds;
    private readonly object _lock = new();
    private long _lastAcceptedAt;
    private bool _hasAccepted;

    public TapDebouncer(TimeSpan? interval = null, Func<long>? clockMilliseconds = null)
    {
        Interval = interval ?? DefaultInterval;
        _clockMilliseconds = clockMilliseconds ?? (() => Environment.TickCount64);
    }

    public TimeSpan Interval { get; }

    /// <summary>True for the first tap and for any tap at least <see cref="Interval"/> after the last accepted one.</summary>
    public bool TryEnter()
    {
        lock (_lock)
        {
            long now = _clockMilliseconds();
            if (_hasAccepted && now - _lastAcceptedAt < Interval.TotalMilliseconds)
            {
                return false;
            }

            _hasAccepted = true;
            _lastAcceptedAt = now;
            return true;
        }
    }
}
