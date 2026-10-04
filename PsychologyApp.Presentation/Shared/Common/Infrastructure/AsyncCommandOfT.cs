using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// <see cref="AsyncCommand"/> with a parameter: a second tap while the first is still running is ignored, so a double tap
/// cannot save, share or delete twice (a plain <c>Command&lt;T&gt;</c> that starts a task has no such guard).
/// </summary>
public sealed class AsyncCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private int _isExecuting;

    public AsyncCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) =>
        Volatile.Read(ref _isExecuting) == 0 && (_canExecute?.Invoke(Convert(parameter)) ?? true);

    public void Execute(object? parameter)
    {
        if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0)
        {
            return;
        }

        _ = ExecuteCoreAsync(Convert(parameter));
    }

    private async Task ExecuteCoreAsync(T? parameter)
    {
        try
        {
            RaiseCanExecuteChanged();
            await _execute(parameter);
        }
        catch (Exception ex)
        {
            if (AsyncCommandExtensions.DefaultErrorHandler is Action<Exception> handler)
            {
                handler.Invoke(ex);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            Volatile.Write(ref _isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() =>
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private static T? Convert(object? parameter) => parameter is T typed ? typed : default;
}
