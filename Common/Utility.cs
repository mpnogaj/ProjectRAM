/*
    Plik zawierający deklaracje przydatnych klas i enumeratorów
*/

using System;
using System.Diagnostics.CodeAnalysis;
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
        Halt,
        Unknown
    }

    /// <summary>
    /// Typ argumentu
    /// </summary>
    public enum ArgumentType
    {
        Label,
        DirectAddress,
        IndirectAddress,
        Const,
        None
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

            if (t == CommandType.Halt)
            {
                this.ArgumentType = ArgumentType.None;
            }
            else
            {
                BigInteger b;
                if (!BigInteger.TryParse(a, out b))
                {
                    if (a.StartsWith('^'))
                        this.ArgumentType = ArgumentType.IndirectAddress;
                    else if (a.StartsWith('='))
                        this.ArgumentType = ArgumentType.Const;
                    else
                        this.ArgumentType = ArgumentType.Label;
                }
                else
                    this.ArgumentType = ArgumentType.DirectAddress;
            }
        }
    }

    /// <summary>
    /// Klasa reprezentującą komórkę pamięci
    /// </summary>
    public class Cell : IComparable<Cell>
    {
        public string Value { get; set; }
        public BigInteger Index { get; }

        public Cell(string v, BigInteger i)
        {
            Value = v;
            Index = i;
        }

        public int CompareTo([AllowNull] Cell other) =>
            other == null ? 1 : Index.CompareTo(other.Index);
    }
}
