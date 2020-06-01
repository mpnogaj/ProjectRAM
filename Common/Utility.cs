/*
    Plik zawierający deklaracje przydatnych klas i enumeratorów
*/

using System.Numerics;

namespace Common
{
    /// <summary>
    /// Typ komendy
    /// </summary>
    public enum CommandType
    {
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

    public enum ArgumentType
    {
        Label,
        DirectAddress,
        IndirectAddress,
        Const
    }

    /// <summary>
    /// Klasa reprezentująca komendę
    /// </summary>
    public class Command
    {
        public ArgumentType ArgumentType { get; set; }
        public CommandType CommandType { get; }
        public string Argument { get; }
        public string Label { get; }
        public string Comment { get; }

        public Command(CommandType t, string a, string l, string c)
        {
            this.CommandType = t;
            this.Argument = a;
            this.Label = l;
            this.Comment = c;
        }
    }

    /// <summary>
    /// Klasa reprezentującą komórkę pamięci
    /// </summary>
    public class Cell
    {
        public string Value { get; set; }
        public BigInteger Index { get; }

        public Cell(string v, BigInteger i)
        {
            Value = v;
            Index = i;
        }
    }
}
