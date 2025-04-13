using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestND.Helpers
{


    // This class allows you to define commands that can be bound to UI elements like buttons
    public class RelayCommand : ICommand
    {
        // The action to perform when the command is executed
        private readonly Action<object> _execute;

        // A function to determine if the command can execute
        private readonly Func<object, bool> _canExecute;

        // Constructor to set up the command with the actions
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            // Ensure the execute action is provided
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Determines if the command can run
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        // The method that gets called when the command is executed
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        // Event that gets raised when the ability to execute changes
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Method to manually trigger the CanExecuteChanged event
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
