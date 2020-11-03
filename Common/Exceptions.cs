using System;

namespace Common
{
    /// <summary>
    /// Special type of exception threw by RAM Interpreter
    /// </summary>
    public class RamInterpreterException : Exception { }

    /// <summary>
    /// Label does not exists in command list. 
    /// Can me verified
    /// </summary>
    public class LabelDoesntExistExcpetion : RamInterpreterException
    {
        private readonly string _message;

        public LabelDoesntExistExcpetion(long line, string label)
        {
            _message = $"Label '{label}' not found. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    /// <summary>
    /// Tried to read number from input tape, but it was empty
    /// Runtime only
    /// </summary>
    public class InputTapeEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public InputTapeEmptyException(long line)
        {
            _message = $"Cannot read an element from input tape because element is empty. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    ///<summary>
    ///Tried to read value from undefined cell
    ///Runtime exception
    ///</summary>
    public class CellDoesntExistException : RamInterpreterException
    {
        private readonly string _message;

        public CellDoesntExistException(long line)
        {
            _message = $"Cannot get cell value because it's undefined. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    /// <summary>
    /// Tried to read value from empty accumulator
    /// Runtime only
    /// </summary>
    public class AccumulatorEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public AccumulatorEmptyException(long line)
        {
            _message = $"Cannot get accumulator value because it's undefined. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    /// <summary>
    /// Argument does not match given command
    /// Can be verified
    /// </summary>
    public class ArgumentIsNotValidException : RamInterpreterException
    {
        private readonly string _message;

        public ArgumentIsNotValidException(long line)
        {
            _message = $"The given argument does not match the command. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }

    }

    /// <summary>
    /// Line is not complete. The command or ragument is missing
    /// Can be verified
    /// </summary>
    public class LineIsEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public LineIsEmptyException(long line)
        {
            _message = $"The line is invalid. Check if a command or argument is missing. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    /// <summary>
    /// The Command Type is unknown
    /// Can be verified
    /// </summary>
    public class UnknownCommandTypeException : RamInterpreterException
    {
        private readonly string _message;

        public UnknownCommandTypeException(long line)
        {
            _message = $"The given command is unknown. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }
}
