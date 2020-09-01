/*
    Plik zawierający deklaracje przydatnych klas i enumeratorów
*/

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// Typ komendy
    /// </summary>
    public enum CommandType
    {
        Null,
        Unknown,
        Add,
        Sub,
        Mult,
        Div,
        Load,
        Store,
        Read,
        Write,
        Jump,
        Jgtz,
        Jzero,
        Halt
    }

    /// <summary>
    /// Typ argumentu
    /// </summary>
    public enum ArgumentType
    {
        //Powinien być, a nie ma
        Null,
        //Nie ma ale jest zbędny (halt)
        None,
        Label,
        DirectAddress,
        IndirectAddress,
        Const
    }

    /// <summary>
    /// Klasa reprezentująca komendę
    /// </summary>
    public class Command : INotifyPropertyChanged
    {
        #region Private variables
        private ArgumentType _argumentType;
        private CommandType _commandType;
        private string _command;
        private string _argument;
        private string _label;
        private string _comment;
        private long _line;
        #endregion

        #region Properties
        public ArgumentType ArgumentType
        {
            get { return _argumentType; }
            set
            {
                _argumentType = value;
                RaisePropertyChangedEvent(nameof(ArgumentType));
            }
        }

        public CommandType CommandType
        {
            get { return _commandType; }
            set
            {
                _commandType = value;
                RaisePropertyChangedEvent(nameof(CommandType));
            }
        }

        public string CommandName
        {
            get { return _command; }
            set
            {
                _command = value;
                if (_command != null && _command != string.Empty)
                {
                    CommandType = (CommandType)Enum.Parse(typeof(CommandType), _command, true);
                }

                RaisePropertyChangedEvent(nameof(CommandName));
            }
        }

        public string Argument
        {
            get { return _argument; }
            set
            {
                _argument = value;
                RaisePropertyChangedEvent(nameof(Argument));
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                RaisePropertyChangedEvent(nameof(Label));
            }
        }

        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChangedEvent(nameof(Comment));
            }
        }

        public long Line
        {
            get { return _line; }
            set
            {
                _line = value;
                RaisePropertyChangedEvent(nameof(Line));
            }
        }
        #endregion

        #region Constructors
        public Command()
        {
            this.ArgumentType = ArgumentType.Null;
            this.CommandType = CommandType.Null;
            this.CommandName = string.Empty;
            this.Argument = string.Empty;
            this.Label = string.Empty;
            this.Comment = string.Empty;
        }

        public Command(string t, string a, string l, string c)
        {
            this.CommandName = t;
            this.Argument = a;
            this.Label = l;
            this.Comment = c;

            if (this.CommandType == CommandType.Halt)
            {
                this.ArgumentType = ArgumentType.None;
            }
            else
            {
                BigInteger b;
                if (!BigInteger.TryParse(a, out b))
                {
                    if (a.StartsWith('^'))
                    {
                        this.ArgumentType = ArgumentType.IndirectAddress;
                    }
                    else if (a.StartsWith('='))
                    {
                        this.ArgumentType = ArgumentType.Const;
                    }
                    else
                    {
                        this.ArgumentType = ArgumentType.Label;
                    }
                }
                else
                {
                    this.ArgumentType = ArgumentType.DirectAddress;
                }
            }
        }

        public Command(string line, long lineNumber)
        {
            line = Regex.Replace(line, @"\s+", " ");
            line = line.Trim();
            string[] words = line.Split(' ');
            int i = 0;

            Line = lineNumber;

            try
            {
                if (words[i].EndsWith(':'))
                {
                    Label = words[i++];
                    Label = Label.Substring(0, Label.Length - 1);
                }
                else
                {
                    Label = string.Empty;
                }

                CommandName = words[i++];

                if (CommandType == CommandType.Halt)
                {
                    Argument = string.Empty;
                    ArgumentType = ArgumentType.None;
                }
                else
                {
                    if (words.Length <= i)
                    {
                        return;
                    }
                    Argument = Regex.Replace(words[i++], @"\t|\n|\r", "");
                    ArgumentType = GetArgumentType(CommandType, Argument);
                }

                Comment = Regex.Replace(String.Join(' ', words, i, words.Length - i), @"\t|\n|\r", "");
                if (Comment != string.Empty)
                {
                    Comment = Comment.Substring(1);
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Methods
        public static CommandType GetCommandType(string command)
        {
            switch (command.ToUpper())
            {
                case "ADD":
                    return CommandType.Add;
                case "SUB":
                    return CommandType.Sub;
                case "MULT":
                    return CommandType.Mult;
                case "DIV":
                    return CommandType.Div;
                case "LOAD":
                    return CommandType.Load;
                case "STORE":
                    return CommandType.Store;
                case "READ":
                    return CommandType.Read;
                case "WRITE":
                    return CommandType.Write;
                case "JUMP":
                    return CommandType.Jump;
                case "JGTZ":
                    return CommandType.Jgtz;
                case "JZERO":
                    return CommandType.Jzero;
                case "HALT":
                    return CommandType.Halt;
                default:
                    return CommandType.Unknown;
            }
        }

        public static ArgumentType GetArgumentType(CommandType command, string argument)
        {
            if (command == CommandType.Halt)
            {
                return ArgumentType.None;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(argument))
                {
                    return ArgumentType.Null;
                }
                if (!BigInteger.TryParse(argument, out _))
                {
                    if (argument.StartsWith('^'))
                    {
                        return ArgumentType.IndirectAddress;
                    }
                    else if (argument.StartsWith('='))
                    {
                        return ArgumentType.Const;
                    }
                    else
                    {
                        return ArgumentType.Label;
                    }
                }
                else
                {
                    return ArgumentType.DirectAddress;
                }
            }
        }
        #endregion

        #region Interface implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }
        #endregion

        public override string ToString()
        {
            string outcome = string.Empty;
            if (!String.IsNullOrWhiteSpace(Label))
            {
                outcome += Label + ": ";
            }
            outcome += CommandType.ToString().ToLower() + " ";
            outcome += Argument + " ";
            if (!String.IsNullOrWhiteSpace(Comment))
            {
                outcome += "#" + Comment;
            }
            return outcome;
        }
    }

    /// <summary>
    /// Klasa reprezentującą komórkę pamięci
    /// </summary>
    public class Cell : IComparable<Cell>
    {
        public string Value { get; set; }
        public string Index { get; }

        public Cell(string v, string i)
        {
            Value = v;
            Index = i;
        }

        public int CompareTo([AllowNull] Cell other) =>
            other == null
                ? 1
                : BigInteger.Compare(BigInteger.Parse(Index), BigInteger.Parse(other.Index));
    }
}
