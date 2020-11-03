using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace RAMEditor.Helpers
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommandBase : IAsyncCommand, ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;

        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly DispatcherTimer _canExecuteChangedTimer;

        public AsyncCommandBase(Func<Task> execute, Func<bool> canExecute)
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

        public bool CanExecute()
        {
            if (_isExecuting)
            {
                return false;
            }
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute();
        }

        public async Task ExecuteAsync()
        {
            if (!_isExecuting)
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync();
        }
    }
}
