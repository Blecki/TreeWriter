using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class FolderDocument : EditableDocument
    {
        public override void Load(string Path)
        {
            this.Path = Path;
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new DirectoryListing(this);
            OpenEditors.Add(r);
            return r;
        }

        public override WeifenLuo.WinFormsUI.Docking.DockState GetPreferredOpeningDockState()
        {
            return WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
        }
    }
}
