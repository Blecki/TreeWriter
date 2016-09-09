using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenDocument : ICommand
    {
        private String FileName;
        private Project Owner;
        public bool Succeeded { get; private set; }

        public OpenDocument(String FileName, Project Owner)
        {
            this.FileName = FileName;
            this.Owner = Owner;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var document = Model.OpenDocument(FileName, Owner);

            if (document == null)
                return;

            if (document.OpenEditors.Count != 0)
                document.OpenEditors[0].BringToFront();
            else
            {
                var docPanel = new DocumentEditor(document, null, Model.SpellChecker, Model.Thesaurus);
                View.OpenControllerPanel(docPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                document.OpenEditors.Add(docPanel);
            }

            Succeeded = true;
        }
    }
}
