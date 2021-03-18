using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAMEditorMultiplatform.Helpers;

namespace RAMEditorMultiplatform.Logic
{
    public static class ButtonLogic
    {
        public static readonly CommandBase NewFileClick = new(() => { Logic.CreateNewPage(); }, null);
        public static readonly CommandBase ExitClick = new(Logic.Exit, null);
    }
}
