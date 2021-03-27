using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEditorMultiplatform.Models
{
    public class MemoryCell
    {
        public string Address { get; set; }
        public string Value { get; set; }

        public MemoryCell()
        {
            Address = string.Empty;
            Value = string.Empty;
        }
    }
}
