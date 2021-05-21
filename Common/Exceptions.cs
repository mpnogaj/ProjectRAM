using System;

namespace Common
{
    /// <summary>
    /// Special type of exception threw by RAM Interpreter
    /// </summary>
    public class RamInterpreterException : Exception
    {
        #region Private Fileds
        
        private readonly long _line;
        
        #endregion
        
        #region Properties
        
        public long Line => _line;
        
        #endregion
        
        #region Constructors
        
        public RamInterpreterException()
        {
        }

        public RamInterpreterException(long line)
        {
            _line = line;
        }
        
        #endregion

        #region Methods
        
        public static string ManualMessage(string lang = "en-GB") => lang switch
        {
            "pl-PL" => "Sprawdź instrukcję po więcej informacji.",
            _ => "Check manual for more information."
        };

        public string LineMessage(string lang = "en-GB") => lang switch
        {
            "pl-PL" => $"Błąd wystąpił w linii: {Line}.",
            _ => $"An error occured in line: {Line}."
        };

        public virtual string LocalizedMessage(string lang) => "";

        public string GetFullLocalizedMessage(string lang) => 
            $"{LocalizedMessage(lang)} {LineMessage(lang)} {ManualMessage(lang)}";
        
        #endregion
    }

    /// <summary>
    /// Label does not exists in command list. 
    /// Can me verified
    /// </summary>
    public class LabelDoesntExistException : RamInterpreterException
    {
        #region Private Fileds
        
        private readonly string _message;
        private readonly string _label;
        
        #endregion
        
        #region Properties
        
        public string Label => _label;
        public override string Message => _message;
        
        #endregion

        #region Constructors

        public LabelDoesntExistException(long line, string label) : base(line)
        {
            _message = $"Label '{label}' not found.";
            _label = label;
        }
        
        #endregion

        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Etykieta '{Label}' nie została znaleziona.",
            _ => _message
        };

        #endregion
    }

    /// <summary>
    /// Tried to read number from input tape, but it was empty
    /// Runtime only
    /// </summary>
    public class InputTapeEmptyException : RamInterpreterException
    {
        #region Private Fields

        private readonly string _message;

        #endregion

        #region Properties

        public override string Message => _message;
        
        #endregion

        #region Constructors

        public InputTapeEmptyException(long line) : base(line)
        {
            _message = $"Cannot read an element from input tape because element is empty.";
        }

        #endregion

        #region Overrides
        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Nie można odczytać elementu z taśmy wejściowej.",
            _ => _message
        };
        #endregion
    }

    ///<summary>
    ///Tried to read value from undefined cell
    ///Runtime exception
    ///</summary>
    public class CellDoesntExistException : RamInterpreterException
    {
        #region Private Fields

        private readonly string _message;

        #endregion

        #region Properties

        public override string Message => _message;

        #endregion

        #region Constructors

        public CellDoesntExistException(long line) : base(line)
        {
            _message = $"Cannot get memory cell value because it's undefined.";
        }

        #endregion

        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Nie można odczytać wartości z komórki pamięci, ponieważ nie jest zainicjalizowana.",
            _ => _message
        };

        #endregion
    }

    /// <summary>
    /// Tried to read value from empty accumulator
    /// Runtime only
    /// </summary>
    public class AccumulatorEmptyException : RamInterpreterException
    {
        #region Private Fields

        private readonly string _message;

        #endregion

        #region Properties
        
        public override string Message => _message;

        #endregion

        #region Constructors

        public AccumulatorEmptyException(long line) : base(line)
        {
            _message = $"Cannot get accumulator value because it's undefined.";
        }

        #endregion

        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Nie można odczytać wartości akumulatora, ponieważ nie jest zainicjalizowany.",
            _ => _message
        };

        #endregion
    }

    /// <summary>
    /// Argument does not match given command
    /// Can be verified
    /// </summary>
    public class ArgumentIsNotValidException : RamInterpreterException
    {
        #region Private Fields

        private readonly string _message;

        #endregion
        
        #region Properties
        
        public override string Message => _message;

        #endregion

        #region Constructors

        public ArgumentIsNotValidException(long line) : base(line)
        {
            _message = $"The given argument does not match the command.";
        }

        #endregion

        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Podany argument nie pasuje do polecenia.",
            _ => _message
        };

        #endregion
    }

    /// <summary>
    /// Line is not complete. The command or ragument is missing
    /// Can be verified
    /// </summary>
    public class LineIsEmptyException : RamInterpreterException
    {

        #region Private Fields

        private readonly string _message;

        #endregion

        #region Properties

        public override string Message => _message;
        
        #endregion
        
        #region Constructors

        public LineIsEmptyException(long line) : base(line)
        {
            _message = $"The line is invalid. Check if a command or argument is missing.";
        }

        #endregion
        
        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Linia jest nieprawidłowa. Sprawdź czy nie brakuje polecania lub argumentu.",
            _ => _message
        };

        #endregion
    }

    /// <summary>
    /// The Command Type is unknown
    /// Can be verified
    /// </summary>
    public class UnknownCommandTypeException : RamInterpreterException
    {
        #region Private Fields

        private readonly string _message;

        #endregion

        #region Properties

        public override string Message => _message;

        #endregion

        #region Constructors

        public UnknownCommandTypeException(long line) : base(line)
        {
            _message = $"The given command is unknown.";
        }

        #endregion

        #region Overrides

        public override string LocalizedMessage(string lang) => lang switch
        {
            "pl-PL" => $"Podane polecenie jest nieznane.",
            _ => _message
        };

        #endregion
    }
}