using System;
using System.Windows.Input;

namespace TourPlanner.WPF.State
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        
        private readonly Action<object> _execute;
        
        public RelayCommand(Action<object> execute)
        {
            this._execute = execute;
            this._canExecute = null;
        }
        
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }
        
        public bool CanExecute(object? parameter)
        {
            return this._canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            this._execute.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}