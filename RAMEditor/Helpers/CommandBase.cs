using System;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace RAMEditor.Helpers
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly DispatcherTimer _canExecuteChangedTimer;

        public CommandBase(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _canExecuteChangedTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1),
            };
            _canExecuteChangedTimer.Tick += _canExecuteChangedTimer_Tick;
            _canExecuteChangedTimer.Start();
        }

        private void _canExecuteChangedTimer_Tick(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
