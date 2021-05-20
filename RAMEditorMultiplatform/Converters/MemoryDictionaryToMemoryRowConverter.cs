using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using RAMEditorMultiplatform.Models;

namespace RAMEditorMultiplatform.Converters
{
    public static class MemoryDictionaryToMemoryRowConverter
    {
        public static ObservableCollection<MemoryRow> MemoryDictionaryToMemoryRows(Dictionary<string, string> memory)
        {
            var orderedMemory = memory.OrderBy(x => x.Key,
                Comparer<string>.Create((x, y) => BigInteger.Compare(BigInteger.Parse(x), BigInteger.Parse(y))));

            ObservableCollection<MemoryRow> newMem = new();
            
            foreach (var (key, val) in orderedMemory)
            {
                newMem.Add(new MemoryRow{Address = key, Value = (string.IsNullOrEmpty(val) ? "?" : val)});
            }

            return newMem;
        }
    }
}