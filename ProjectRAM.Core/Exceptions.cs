using System;
using System.Diagnostics.CodeAnalysis;
using ProjectRAM.Core.Properties;

namespace ProjectRAM.Core
{
	/// <summary>
	/// Tried to read value from empty accumulator
	/// Runtime only
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class AccumulatorEmptyException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public AccumulatorEmptyException(long line) : base(line)
		{
			_message = Strings.accumulatorUninitializedMessage;
		}

		#endregion Constructors
	}

	/// <summary>
	/// Argument does not match given command
	/// Can be verified
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class ArgumentIsNotValidException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public ArgumentIsNotValidException(long line) : base(line)
		{
			_message = Strings.invalidArgumentMessage;
		}

		#endregion Constructors
	}

	///<summary>
	///Tried to read value from undefined cell
	///Runtime exception
	///</summary>
	[ExcludeFromCodeCoverage]
	public class CellDoesntExistException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public CellDoesntExistException(long line) : base(line)
		{
			_message = Strings.cellUninitializedMessage;
		}

		#endregion Constructors
	}

	/// <summary>
	/// Tried to read number from input tape, but it was empty
	/// Runtime only
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class InputTapeEmptyException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public InputTapeEmptyException(long line) : base(line)
		{
			_message = Strings.inputTapeEmptyMessage;
		}

		#endregion Constructors
	}

	/// <summary>
	/// Label does not exists in command list.
	/// Can me verified
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class LabelDoesntExistException : RamInterpreterException
	{
		#region Private Fileds

		private readonly string _label;
		private readonly string _message;

		#endregion Private Fileds

		#region Properties

		public string Label => _label;
		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public LabelDoesntExistException(long line, string label) : base(line)
		{
			_message = string.Format(Strings.labelDoesntExistMessage, label);
			_label = label;
		}

		#endregion Constructors
	}

	/// <summary>
	/// Line is not complete. The command or ragument is missing
	/// Can be verified
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class LineIsEmptyException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public LineIsEmptyException(long line) : base(line)
		{
			_message = Strings.invalidaLineMessage;
		}

		#endregion Constructors
	}

	/// <summary>
	/// Special type of exception threw by RAM Interpreter
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class RamInterpreterException : Exception
	{
		#region Private Fileds

		private readonly long _line;

		#endregion Private Fileds

		#region Properties

		public long Line => _line;

		#endregion Properties

		#region Constructors

		public RamInterpreterException(long line)
		{
			_line = line;
		}

		#endregion Constructors

		#region Methods

		public static string ManualMessage() => Strings.manualMessage;

		/// <summary>
		/// Get full exception message based on current culture. Change culture by setting CurrentCulture property in Settings class
		/// </summary>
		/// <returns>Full exception message based on current culture</returns>
		public string GetFullLocalizedMessage() =>
			$"{LocalizedMessage()} {LineMessage()} {ManualMessage()}";

		public string LineMessage() => string.Format(Strings.lineMessage, Line);

		public virtual string LocalizedMessage() => string.Empty;

		#endregion Methods
	}

	/// <summary>
	/// The Command Type is unknown
	/// Can be verified
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class UnknownCommandTypeException : RamInterpreterException
	{
		#region Private Fields

		private readonly string _message;

		#endregion Private Fields

		#region Properties

		public override string Message => _message;

		#endregion Properties

		#region Constructors

		public UnknownCommandTypeException(long line) : base(line)
		{
			_message = Strings.unknownCommandMessage;
		}

		#endregion Constructors
	}
}