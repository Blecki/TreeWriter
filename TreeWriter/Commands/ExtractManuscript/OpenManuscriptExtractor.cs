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
        private ManuscriptDocument Document;
        public bool Succeeded { get; private set; }

        public OpenManuscriptExtractor(ManuscriptDocument Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            ManuscriptExtractor existing = Document.OpenEditors.FirstOrDefault(v => v is ManuscriptExtractor) as ManuscriptExtractor;
            if (existing == null)
            {
                existing = new ManuscriptExtractor(Document);
                View.OpenControllerPanel(existing, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
                Document.OpenEditors.Add(existing);
            }
            existing.Focus();
            Succeeded = true;
        }
    }
}
