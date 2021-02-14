using System;
using System.ComponentModel;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// Class whitch represents single command
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
        /// <summary>
        /// Type of argument
        /// </summary>
        public ArgumentType ArgumentType
        {
            get { return _argumentType; }
            set
            {
                _argumentType = value;
                RaisePropertyChangedEvent(nameof(ArgumentType));
            }
        }

        /// <summary>
        /// Type of command
        /// </summary>
        public CommandType CommandType
        {
            get { return _commandType; }
            set
            {
                _commandType = value;
                RaisePropertyChangedEvent(nameof(CommandType));
            }
        }

        /// <summary>
        /// Command
        /// </summary>
        public string CommandName
        {
            get { return _command; }
            set
            {
                _command = value;
                if (_command != null && _command != string.Empty)
                {
                    CommandType = GetCommandType(value);
                    /*object type;
                    if (Enum.TryParse(typeof(CommandType), _command, true, out type))
                    {
                        CommandType = (CommandType)type;
                    }
                    else
                    {
                        CommandType = CommandType.Unknown;
                    }*/
                    //CommandType = (CommandType)Enum.Parse(typeof(CommandType), _command, true);
                }

                RaisePropertyChangedEvent(nameof(CommandName));
            }
        }

        /// <summary>
        /// Argument
        /// </summary>
        public string Argument
        {
            get { return _argument; }
            set
            {
                _argument = value;
                if(_argument != null && _argument != string.Empty)
                {
                    _argument = Regex.Replace(_argument, @"#|\s", "");
                    ArgumentType = GetArgumentType(_argument);
                }
                RaisePropertyChangedEvent(nameof(Argument));
            }
        }

        /// <summary>
        /// Label
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                if(_label != null && _label != string.Empty)
                {
                    _label = Regex.Replace(_label, @"#|\s", "");
                }
                RaisePropertyChangedEvent(nameof(Label));
            }
        }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                RaisePropertyChangedEvent(nameof(Comment));
            }
        }

        /// <summary>
        /// Line number
        /// </summary>
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
        /// <summary>
        /// Creates empty command
        /// </summary>
        public Command()
        {
            this.Argument = string.Empty;
            this.ArgumentType = ArgumentType.Null;
            this.CommandName = string.Empty;
            this.CommandType = CommandType.Null;
            this.Comment = string.Empty;
            this.Label = string.Empty;      
        }

        /// <summary>
        /// Creates command from given arguments, without line
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="argument">Argument</param>
        /// <param name="label">Label</param>
        /// <param name="comment">Comment</param>
        public Command(string command, string argument, string label, string comment)
        {
            this.CommandName = command;
            this.Argument = argument;
            this.Label = label;
            this.Comment = comment;
            this.ArgumentType = GetArgumentType(Argument);
        }

        /// <summary>
        /// Creates command from line. Requires line number
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="lineNumber">Line number</param>
        public Command(string line, long lineNumber)
        {
            line = Regex.Replace(line, @"\s+", " ");
            line = line.Trim();
            string[] words = line.Split(' ');
            int i = 0;

            Line = lineNumber;
            Label = string.Empty;
            CommandName = string.Empty;
            Argument = string.Empty;
            ArgumentType = ArgumentType.Null;
            Comment = string.Empty;
            try
            {
                if (!words[i].StartsWith("#"))
                {
                    if (words[i].EndsWith(':'))
                    {
                        Label = words[i++];
                        Label = Label.Substring(0, Label.Length - 1);
                    }

                    if (!int.TryParse(words[i], out _))
                    {
                        CommandName = words[i++];
                    }

                    if (CommandType == CommandType.Halt)
                    {
                        ArgumentType = ArgumentType.Null;
                    }
                    else if (CommandType == CommandType.Unknown)
                    {
                        string a = CommandName;
                        CommandName = string.Empty;
                        if (a.StartsWith('#'))
                        {
                            Comment = Regex.Replace(a, @"\t |\n |\r |#", "");
                            return;
                        }
                        Argument = a;
                    }
                    else
                    {
                        if (words.Length <= i)
                        {
                            return;
                        }
                        Argument = Regex.Replace(words[i++], @"\t|\n|\r", "");
                        ArgumentType = GetArgumentType(Argument);
                    }
                }
                if (words.Length - 1 > 0)
                {
                    Comment = Regex.Replace(String.Join(' ', words, i, words.Length - i), @"\t|\n|\r|#", "");
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get type of command
        /// </summary>
        /// <param name="command">Command text</param>
        /// <returns>Command type</returns>
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

        /// <summary>
        /// Get type of argument
        /// </summary>
        /// <param name="argument">Argument text</param>
        /// <returns></returns>
        public static ArgumentType GetArgumentType(string argument)
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

        public string FormatedArg()
        {
            string arg = this.Argument;
            if (this.ArgumentType == ArgumentType.Const || this.ArgumentType == ArgumentType.IndirectAddress)
            {
                return arg.Substring(1);
            }
            return arg;
        }

        /// <summary>
        /// ToString() method override
        /// </summary>
        /// <returns>{label}: {command} {argument} #{comment}</returns>
        public override string ToString()
        {
            string outcome = string.Empty;
            if (!String.IsNullOrWhiteSpace(Label))
            {
                outcome += Label + ": ";
            }
            if (CommandType != CommandType.Null && CommandType != CommandType.Unknown)
            {
                outcome += CommandType.ToString().ToLower() + " ";
            }
            outcome += Argument + " ";
            if (!String.IsNullOrWhiteSpace(Comment))
            {
                outcome += "#" + Comment;
            }
            return outcome;
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
    }
}
