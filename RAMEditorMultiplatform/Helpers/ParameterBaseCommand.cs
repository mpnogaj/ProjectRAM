﻿using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RAMEditorMultiplatform.Helpers
{
    public class ParameterBaseCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;
        private readonly DispatcherTimer _canExecuteChangedTimer;

        public ParameterBaseCommand(Action<T> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _canExecuteChangedTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1),
            };
            _canExecuteChangedTimer.Tick += CanExecuteChangedTimer_Tick;
            _canExecuteChangedTimer.Start();
        }

        private void CanExecuteChangedTimer_Tick(object sender, EventArgs e)
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
            _execute((T)parameter);
        }
    }
}