using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ProjectRAM.Core.Properties;

namespace ProjectRAM.Core;

/// <summary>
/// Tried to read value from empty accumulator
/// Runtime only
/// </summary>
[ExcludeFromCodeCoverage]
public class AccumulatorEmptyException : RamInterpreterException
{
	public override string Message { get; }

	public AccumulatorEmptyException(long line) : base(line)
	{
		Message = Resources.ResourceManager.GetString(nameof(Resources.accumulatorUninitializedMessage),
			CultureInfo.InvariantCulture) ?? throw new Exception();
	}

	public override string LocalizedMessage()
	{
		return Resources.accumulatorUninitializedMessage;
	}
}

/// <summary>
/// Argument does not match given command
/// Can be verified
/// </summary>
[ExcludeFromCodeCoverage]
public class ArgumentIsNotValidException : RamInterpreterException
{
	public override string Message { get; }

	public ArgumentIsNotValidException(long line) : base(line)
	{
		Message = Resources.ResourceManager.GetString(nameof(Resources.invalidArgumentMessage),
			CultureInfo.InvariantCulture) ?? throw new Exception(); 
	}

	public override string LocalizedMessage()
	{
		return Resources.invalidArgumentMessage;
	}
}

///<summary>
///Tried to read value from undefined cell
///Runtime exception
///</summary>
[ExcludeFromCodeCoverage]
public class UninitializedCellException : RamInterpreterException
{
	public override string Message { get; }
		
	public UninitializedCellException(long line) : base(line)
	{
		Message = Resources.cellUninitializedMessage;
	}

	public override string LocalizedMessage()
	{
		return Resources.cellUninitializedMessage;
	}
}

/// <summary>
/// Tried to read number from input tape, but it was empty
/// Runtime only
/// </summary>
[ExcludeFromCodeCoverage]
public class InputTapeEmptyException : RamInterpreterException
{
	public override string Message { get; }

	public InputTapeEmptyException(long line) : base(line)
	{
		Message = Resources.ResourceManager.GetString(nameof(Resources.inputTapeEmptyMessage),
			CultureInfo.InvariantCulture) ?? throw new Exception();
	}

	public override string LocalizedMessage()
	{
		return Resources.inputTapeEmptyMessage;
	}
}

/// <summary>
/// Label does not exists in command list.
/// Can me verified
/// </summary>
[ExcludeFromCodeCoverage]
public class UnknownLabelException : RamInterpreterException
{
	private string Label { get; }
	public override string Message { get; }

	public UnknownLabelException(long line, string label) : base(line)
	{
		Message = string.Format(Resources.ResourceManager.GetString(nameof(Resources.labelDoesntExistMessage), 
			CultureInfo.InvariantCulture) ?? throw new Exception(), label);
		Label = label;
	}

	public override string LocalizedMessage()
	{
		return string.Format(Resources.labelDoesntExistMessage, Label);
	}
}

/// <summary>
/// Line is not complete. The command or argument is missing
/// Can be verified
/// </summary>
[ExcludeFromCodeCoverage]
public class LineIsInvalidException : RamInterpreterException
{
	public override string Message { get; }

	public LineIsInvalidException(long line) : base(line)
	{
		Message = Resources.ResourceManager.GetString(nameof(Resources.invalidaLineMessage), 
			CultureInfo.InvariantCulture) ?? throw new Exception();
	}

	public override string LocalizedMessage()
	{
		return Resources.invalidaLineMessage;
	}
}

/// <summary>
/// The Command Type is unknown
/// Can be verified
/// </summary>
[ExcludeFromCodeCoverage]
public class UnknownCommandTypeException : RamInterpreterException
{
	public override string Message { get; }

	public UnknownCommandTypeException(long line) : base(line)
	{
		Message = Resources.ResourceManager.GetString(nameof(Resources.unknownCommandMessage),
			CultureInfo.InvariantCulture) ?? throw new Exception();
	}

	public override string LocalizedMessage()
	{
		return Resources.unknownCommandMessage;
	}
}

[ExcludeFromCodeCoverage]
public class LabelIsNotValidException : RamInterpreterException
{
	public override string Message { get; }

	public LabelIsNotValidException(long line) : base(line)
	{
		Message = "Label is not valid";
	}
}

[ExcludeFromCodeCoverage]
public class DivByZero : RamInterpreterException
{
	public override string Message { get; }

	public DivByZero(long line) : base(line)
	{
		Message = "Attempted to divide by 0";
	}
}

[ExcludeFromCodeCoverage]
public class ValueIsNaN : RamInterpreterException
{
	public override string Message { get; }

	public ValueIsNaN(long line) : base(line)
	{
		Message = "Value is not a number";
	}
}

/// <summary>
/// Special type of exception threw by RAM Interpreter
/// </summary>
[ExcludeFromCodeCoverage]
public class RamInterpreterException : Exception
{
	public long Line { get; }

	protected RamInterpreterException(long line)
	{
		Line = line;
	}

	private static string ManualMessage() => Resources.manualMessage;
	private string LineMessage() => string.Format(Resources.lineMessage, Line);

	/// <summary>
	/// Get full exception message based on current culture. Change culture by setting CurrentCulture property in Settings class
	/// </summary>
	/// <returns>Full exception message based on current culture</returns>
	public string GetFullLocalizedMessage() =>
		$"{LocalizedMessage()} {LineMessage()} {ManualMessage()}";

	public virtual string LocalizedMessage() => Message;
}