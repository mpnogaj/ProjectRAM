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
        Null,
        None,
        Label,
        DirectAddress,
        IndirectAddress,
        Const
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
