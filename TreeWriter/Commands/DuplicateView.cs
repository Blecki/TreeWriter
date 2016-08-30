using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DuplicateView :ICommand
    {
        private Document Document;

        public DuplicateView(Document Document)
        {
            this.Document = Document;
        }

        public void Execute(Model Model, Main View)
        {
            var existingScintillaDocument = Document.OpenEditors[0].GetScintillaDocument();
            var docPanel = new DocumentEditor(Document, existingScintillaDocument, Model.SpellChecker, Model.Thesaurus);
            View.OpenControllerPanel(docPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            Document.OpenEditors.Add(docPanel);
            docPanel.Focus();
        }
    }
}
