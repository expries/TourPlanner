using System;
using System.Windows.Input;

namespace TourPlanner.State
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        
        private readonly Action<object> _execute;
        
        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
            _canExecute = null;
        }
        
        public RelayCommand(Action<object> execute, Predicate<object> canExecute )
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}