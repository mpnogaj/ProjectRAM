using System.Collections.ObjectModel;

namespace ProjectRAM.Core.Models;

public readonly record struct InterpreterResult(ReadOnlyCollection<Cell> Memory, ComplexityReport ComplexityReport);
