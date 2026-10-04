using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// A synchronous command that ignores a second invocation within the debounce interval. For steps that complete instantly
/// (wizard Next/Back, onboarding), where <see cref="AsyncCommand"/>'s "still running" guard cannot tell a double tap apart.
/// </summary>
public sealed class DebouncedCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    private readonly TapDebouncer _debouncer;

    public DebouncedCommand(Action execute, Func<bool>? canExecute = null, TapDebouncer? debouncer = null)
    {
        _execute = execute;
        _canExecute = canExecute;
        _debouncer = debouncer ?? new TapDebouncer();
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter)
    {
        if (_canExecute is not null && !_canExecute())
        {
            return;
        }

        if (_debouncer.TryEnter())
        {
            _execute();
        }
    }

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
