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
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            //TODO: This should use document.OpenView. OpenView should handle linking scintilla documents.
            if (Document.OpenEditors.Count < 1) return;
            var documentView = Document.OpenEditors[0] as DocumentEditor;
            if (documentView == null) return;
            var existingScintillaDocument = documentView.GetScintillaDocument();
            var docPanel = new DocumentEditor(Document, existingScintillaDocument, Model.SpellChecker, Model.Thesaurus);
            View.OpenControllerPanel(docPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            Document.OpenEditors.Add(docPanel);
            docPanel.Focus();
        }
    }
}
