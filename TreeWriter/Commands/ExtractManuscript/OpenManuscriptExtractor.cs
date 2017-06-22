using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF.Commands.Extract
{
    public class OpenManuscriptExtractor : ICommand
    {
        private String Path;
        public bool Succeeded { get; private set; }

        public OpenManuscriptExtractor(String Path)
        {
            this.Path = Path;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var existing = new ManuscriptExtractor(Path);
            View.OpenControllerPanel(existing, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            existing.Focus();
            Succeeded = true;
        }
    }
}
