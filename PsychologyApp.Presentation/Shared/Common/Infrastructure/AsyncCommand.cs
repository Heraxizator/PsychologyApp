using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.Common;

public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private int _isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        Volatile.Read(ref _isExecuting) == 0 && (_canExecute?.Invoke() ?? true);

    public void Execute(object? parameter)
    {
        if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0)
        {
            return;
        }

        _ = ExecuteCoreAsync();
    }

    private async Task ExecuteCoreAsync()
    {
        try
        {
            RaiseCanExecuteChanged();
            await _execute().ConfigureAwait(false);
        }
        finally
        {
            Volatile.Write(ref _isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
