using System;
using System.Windows.Input;
using Avalonia.Threading;

namespace ProjectRAM.Editor.ViewModels.Commands
{
	public class RelayCommand<T> : ICommand
	{
		#region Fields

		public event EventHandler? CanExecuteChanged;
		private readonly Action<T> _execute;
		private readonly Func<bool> _canExecute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="RelayCommand{T}"/>.
		/// </summary>
		/// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
		public RelayCommand(Action<T> execute) : this(execute, () => true) { }

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Func<bool> canExecute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;

			DispatcherTimer canExecuteChangedTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 50),
			};
			canExecuteChangedTimer.Tick += CanExecuteChangedTimer_Tick;
			canExecuteChangedTimer.Start();
		}

		#endregion

		#region ICommand Members

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		public bool CanExecute(object? parameter)
		{
			return _canExecute();
		}

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
		public void Execute(object? parameter)
		{
			if (parameter == null || parameter.GetType() != typeof(T))
				throw new InvalidOperationException();
			_execute((T)parameter);
		}

		private void CanExecuteChangedTimer_Tick(object? sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}

	public class RelayCommand : ICommand
	{
		#region Fields

		public event EventHandler? CanExecuteChanged;
		private readonly Action _execute;
		private readonly Func<bool>? _canExecute;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="RelayCommand{T}"/>.
		/// </summary>
		/// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
		public RelayCommand(Action execute) : this(execute, () => true) { }

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
			DispatcherTimer canExecuteChangedTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 50),
			};
			canExecuteChangedTimer.Tick += CanExecuteChangedTimer_Tick;
			canExecuteChangedTimer.Start();
		}

		#endregion

		#region ICommand Members

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		public bool CanExecute(object? parameter)
		{
			return _canExecute == null || _canExecute();
		}

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
		public void Execute(object? parameter)
		{
			_execute();
		}


		private void CanExecuteChangedTimer_Tick(object? sender, EventArgs e)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}
