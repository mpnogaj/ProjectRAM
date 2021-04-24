using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEditorMultiplatform.Helpers
{
    public static class Constant
    {
        public static readonly List<string> AvailableCommands = new List<string>
        {
            "load",
            "store",
            "read",
            "write",
            "add",
            "sub",
            "mult",
            "div",
            "jump",
            "jzero",
            "jgtz",
            "halt"
        };
    }
}
