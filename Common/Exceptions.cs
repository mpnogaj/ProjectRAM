using System;

namespace Common
{
    public class RamInterpreterException : Exception { }

    //Nie ma takiej etykiety
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

    //Taśma wejścia pusta
    //Runtime exception
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

    //Nie ma takiej komórki
    //Runtime exception
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

    //Akumulator pusty
    //Runtime exception
    public class AcumulatorEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public AcumulatorEmptyException(long line)
        {
            _message = $"Cannot get accumulator value because it's undefined. An error occured in line: {line}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    //argument nie poprwny
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

    //Linia jest nieprawidlowa (brak komendy lub argumentu)
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
}
