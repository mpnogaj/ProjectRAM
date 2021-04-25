using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Models;

namespace RAMEditorMultiplatform.Converters
{
    public static class MemoryRowToStringConverter
    {
        public static string MemoryRowsToString(List<MemoryRow> input)
        {
            string output = string.Empty;
            foreach(var row in input)
            {
                output += row.ToString() + '\n';
            }
            return output;
        }
    }
}
