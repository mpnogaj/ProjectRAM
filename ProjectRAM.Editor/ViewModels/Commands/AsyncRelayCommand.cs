using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;

namespace ProjectRAM.Editor.ViewModels.Commands
{
	public class AsyncRelayCommand<T> : IAsyncCommand<T>
	{
		public event EventHandler? CanExecuteChanged;
		private readonly Func<T, Task> _execute;
		private readonly Func<bool>? _canExecute;
		private bool _isExecuting;

		public AsyncRelayCommand(Func<T, Task> execute, Func<bool>? canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
			var canExecuteChangedTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 50),
			};
			canExecuteChangedTimer.Tick += CanExecuteChangedTimer_Tick;
			canExecuteChangedTimer.Start();
		}

		private void CanExecuteChangedTimer_Tick(object? sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute(T parameter)
		{
			return !_isExecuting && (_canExecute == null || _canExecute());
		}

		public async Task ExecuteAsync(T parameter)
		{
			if (!_isExecuting)
			{
				_isExecuting = true;
				await _execute(parameter);
				_isExecuting = false;
			}
		}

		#region ICommand implementation

		bool ICommand.CanExecute(object? parameter)
		{
			return parameter is T param && CanExecute(param);
		}

		void ICommand.Execute(object? parameter)
		{
			if (parameter is not T param) return;
			_ = ExecuteAsync(param);
		}

		#endregion
	}

	public class AsyncRelayCommand : IAsyncCommand
	{
		public event EventHandler? CanExecuteChanged;
		private readonly Func<Task> _execute;
		private readonly Func<bool>? _canExecute;
		private bool _isExecuting;
		
		public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
			var canExecuteChangedTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 50),
			};
			canExecuteChangedTimer.Tick += CanExecuteChangedTimer_Tick;
			canExecuteChangedTimer.Start();
		}

		private void CanExecuteChangedTimer_Tick(object? sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool CanExecute()
		{
			return !_isExecuting && (_canExecute == null || _canExecute());
		}

		public async Task ExecuteAsync()
		{
			if (!_isExecuting)
			{
				_isExecuting = true;
				await _execute();
				_isExecuting = false;
			}
		}

		#region ICommand implementation

		bool ICommand.CanExecute(object? parameter)
		{
			return CanExecute();
		}

		void ICommand.Execute(object? parameter)
		{
			_ = ExecuteAsync();
		}

		#endregion
	}
}
