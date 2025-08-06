using System.Windows.Input;

namespace VSLibrary.Common.MVVM.Core;

/// <summary>
/// Represents a relay command implementation for commands that do not require parameters.
/// </summary>
public class RelayCommand : ICommand
{
    /// <summary>
    /// The action to execute when the command is invoked.
    /// </summary>
    private readonly Action _execute;


    /// <summary>
    /// A delegate that determines whether the command can execute.
    /// </summary>
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute when the command is invoked.</param>
    /// <param name="canExecute">
    /// A function that determines whether the command can execute.  
    /// If null, the command is always considered executable.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute"/> is null.</exception>
    public RelayCommand(Action execute, Func<bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (() => true);
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// An optional parameter passed by the command source.  
    /// This implementation does not use the parameter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the command can execute; otherwise, <c>false</c>.
    /// </returns>
    public bool CanExecute(object? parameter) => _canExecute();

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="parameter">
    /// An optional parameter passed by the command source.  
    /// This implementation does not use the parameter.
    /// </param>
    public void Execute(object? parameter) => _execute();

    /// <summary>
    /// Raises the CanExecuteChanged event to update the command’s executable state.
    /// Forces the UI to refresh and re-evaluate the command’s availability.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// Represents a command that can be executed with a parameter of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the parameter required to execute the command.</typeparam>
public class RelayCommand<T> : ICommand
{
    /// <summary>
    /// The action to execute when the command is invoked.
    /// </summary>
    private readonly Action<T> _execute;

    /// <summary>
    /// A function that determines whether the command can execute with the given parameter.
    /// </summary>
    private readonly Func<T, bool> _canExecute;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler ? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
    /// </summary>
    /// <param name="execute">An <see cref="Action{T}"/> that defines the method to execute.</param>
    /// <param name="canExecute">
    /// A <see cref="Func{T, Boolean}"/> that determines whether the command can execute.
    /// If null, the command is always considered executable.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute"/> is null.</exception>
    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (_ => true);
    }

    /// <summary>
    /// Determines whether the command can execute with the given parameter.
    /// </summary>
    /// <param name="parameter">The parameter required to evaluate command executability.</param>
    /// <returns>
    /// A boolean value indicating whether the command can execute.
    /// Returns false if the parameter is not of type <typeparamref name="T"/>.
    /// </returns>
    public bool CanExecute(object? parameter) // Updated to match the nullability of the interface
    {
        return parameter is T value && _canExecute(value);
    }

    /// <summary>
    /// Executes the command with the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter required to execute the command. Must be of type <typeparamref name="T"/>.</param>
    public void Execute(object? parameter)
    {
        if (parameter is T value)
        {
            _execute(value);
        }
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event to notify the UI that the command's execution status has changed.
    /// Forces the UI to re-evaluate whether the command can execute.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        CommandManager.InvalidateRequerySuggested();
    }
}
