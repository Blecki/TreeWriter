using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenFile :ICommand
    {
        private String FileName;
        private Project Owner;

        public OpenFile(String FileName, Project Owner)
        {
            this.FileName = FileName;
            this.Owner = Owner;
        }

        public void Execute(Model Model, Main View)
        {
            var document = Model.OpenDocument(FileName, Owner);
            if (document.OpenEditors.Count != 0)
                document.OpenEditors[0].BringToFront();
            else
            {
                // We would pass the existing scintilla document of an open editor to implement multiple views.
                var docPanel = new DocumentEditor(document, null);
                View.OpenControllerPanel(docPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                document.OpenEditors.Add(docPanel);
            }
        }
    }
}
