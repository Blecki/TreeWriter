using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DuplicateView :ICommand
    {
        private EditableDocument Document;
        public bool Succeeded { get; private set; }

        public DuplicateView(EditableDocument Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (Document.OpenEditors.Count < 1) return;
            var docPanel = Document.OpenView(Model);
            View.OpenControllerPanel(docPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            docPanel.Focus();
            Succeeded = true;
        }
    }
}
