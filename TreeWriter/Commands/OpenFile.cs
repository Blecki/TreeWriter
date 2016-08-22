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

        public OpenFile(String FileName)
        {
            this.FileName = FileName;
        }

        public void Execute(ProjectModel Model, Main View)
        {
            var document = Model.OpenDocument(FileName);
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
