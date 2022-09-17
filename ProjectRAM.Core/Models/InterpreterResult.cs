using System.Collections.ObjectModel;

namespace ProjectRAM.Core.Models;

public readonly record struct InterpreterResult(ReadOnlyDictionary<string, string> Memory, ComplexityReport ComplexityReport);
