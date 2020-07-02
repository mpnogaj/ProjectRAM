using System;

namespace Common
{
    public class RamInterpreterException : Exception { }
    //Nie ma takiej etykiety
    public class LabelDoesntExistExcpetion : RamInterpreterException
    {
        private string _message;

        public LabelDoesntExistExcpetion(int line)
        {
            _message = $"Label not found. An error occured in line: {line + 1}";
        }

        public override string Message 
        {
            get => _message;
        }
    }

    //Taśma wejścia pusta
    public class InputTapeEmptyException : RamInterpreterException
    {
        private string _message;

        public InputTapeEmptyException(int line)
        {
            _message = $"Cannot read an element from input tape because element is empty. An error occured in line: {line + 1}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    //Nie ma takiej komórki
    public class CellDoesntExistException : RamInterpreterException
    {
        private string _message;

        public CellDoesntExistException(int line)
        {
            _message = $"Cannot get cell value because it's undefined. An error occured in line: {line + 1}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    //Akumulator pusty
    public class AcumulatorEmptyException : RamInterpreterException
    {
        private string _message;

        public AcumulatorEmptyException(int line)
        {
            _message = $"Cannot get accumulator value because it's undefined. An error occured in line: {line + 1}";
        }

        public override string Message
        {
            get => _message;
        }
    }

    //argument nie poprwny
    public class ArgumentIsNotValidException : RamInterpreterException
    {
        private string _message;

        public ArgumentIsNotValidException(int line)
        {
            _message = $"The given argument does not match . An error occured in line: {line + 1}";
        }

        public override string Message
        {
            get => _message;
        }

    }
}
