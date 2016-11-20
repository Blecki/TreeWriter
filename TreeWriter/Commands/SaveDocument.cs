using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class SaveDocument : ICommand
    {
        private EditableDocument Document;
        public bool Succeeded { get; private set; }

        public SaveDocument(EditableDocument Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            Document.SaveDocument();
            Succeeded = true;
        }
    }
}
