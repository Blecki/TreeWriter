using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class SaveDocument : ICommand
    {
        private Document Document;
        public bool Succeeded { get; private set; }

        public SaveDocument(Document Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (!Document.NeedChangesSaved) return;
            Model.SaveDocument(Document);
            foreach (var editor in Document.OpenEditors) editor.UpdateTitle();
            Succeeded = true;
        }
    }
}
