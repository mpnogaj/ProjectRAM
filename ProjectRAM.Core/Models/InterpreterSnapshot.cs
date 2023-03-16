using System.Collections.ObjectModel;

namespace ProjectRAM.Core.Models;

public readonly record struct InterpreterSnapshot(ReadOnlyCollection<Cell> Memory, ComplexityReport ComplexityReport);
