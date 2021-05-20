using System;

namespace Common
{
    /// <summary>
    /// Special type of exception threw by RAM Interpreter
    /// </summary>
    public class RamInterpreterException : Exception
    {
        private readonly long _line;
        public long Line => _line;

        public RamInterpreterException()
        {
        }

        public RamInterpreterException(long line)
        {
            _line = line;
        }

        public static string ManualMessage(string lang = "en-GB") => 
            lang switch
            {
                "pl-PL" => "Sprawdź instrukcję po więcej informacji",
                _ => "Check manual for more information"
            };

        public string LineMessage(string lang = "en-GB") => 
            lang switch
            {
                "pl-PL" => $"Błąd wystąpił w linii: {Line}",
                _ => $"An error occured in line: {Line}"
            };

        public virtual string LocalizedMessage(string lang) => "";

        public string GetFullLocalizedMessage(string lang) =>
            $"{LocalizedMessage(lang)}. {LineMessage(lang)}. {ManualMessage(lang)}";
    }

    /// <summary>
    /// Label does not exists in command list. 
    /// Can me verified
    /// </summary>
    public class LabelDoesntExistException : RamInterpreterException
    {
        private readonly string _message;
        private readonly string _label;

        public string Label => _label;

        public override string Message => _message;

        public LabelDoesntExistException(long line, string label) : base(line)
        {
            _message = $"Label '{label}' not found";
            _label = label;
        }

        public override string LocalizedMessage(string lang) =>
            lang switch
            {
                "pl-PL" => $"Etykieta '{Label}' nie została znaleziona.",
                _ => _message
            };
    }

    /// <summary>
    /// Tried to read number from input tape, but it was empty
    /// Runtime only
    /// </summary>
    public class InputTapeEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public InputTapeEmptyException(long line) : base(line)
        {
            _message =
                $"Cannot read an element from input tape because element is empty. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg = $"Nie można odczytać elementu z taśmy wejściowej. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }

    ///<summary>
    ///Tried to read value from undefined cell
    ///Runtime exception
    ///</summary>
    public class CellDoesntExistException : RamInterpreterException
    {
        private readonly string _message;

        public CellDoesntExistException(long line) : base(line)
        {
            _message = $"Cannot get memory cell value because it's undefined. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg =
                        $"Nie można odczytać wartości z komórki pamięci, ponieważ nie jest zainicjalizowana. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }

    /// <summary>
    /// Tried to read value from empty accumulator
    /// Runtime only
    /// </summary>
    public class AccumulatorEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public AccumulatorEmptyException(long line) : base(line)
        {
            _message = $"Cannot get accumulator value because it's undefined. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg =
                        $"Nie można odczytać wartości akumulatora, ponieważ nie jest zainicjalizowany. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }

    /// <summary>
    /// Argument does not match given command
    /// Can be verified
    /// </summary>
    public class ArgumentIsNotValidException : RamInterpreterException
    {
        private readonly string _message;

        public ArgumentIsNotValidException(long line) : base(line)
        {
            _message = $"The given argument does not match the command. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg = $"Podany argument nie pasuje do polecenia. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }

    /// <summary>
    /// Line is not complete. The command or ragument is missing
    /// Can be verified
    /// </summary>
    public class LineIsEmptyException : RamInterpreterException
    {
        private readonly string _message;

        public LineIsEmptyException(long line) : base(line)
        {
            _message =
                $"The line is invalid. Check if a command or argument is missing. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg =
                        $"Linia jest nieprawidłowa. Sprawdź czy nie brakuje polecania lub argumentu. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }

    /// <summary>
    /// The Command Type is unknown
    /// Can be verified
    /// </summary>
    public class UnknownCommandTypeException : RamInterpreterException
    {
        private readonly string _message;

        public UnknownCommandTypeException(long line) : base(line)
        {
            _message = $"The given command is unknown. An error occured in line: {line}.";
        }

        public override string Message
        {
            get => _message;
        }

        public override string LocalizedMessage(string lang)
        {
            string msg;
            switch (lang)
            {
                case "pl-PL":
                    msg = $"Podane polecenie jest nieznane. Błąd wystąpił w lini: {Line}.";
                    break;
                default:
                    msg = Message;
                    break;
            }

            return $"{msg} {ManualMessage(lang)}";
        }
    }
}