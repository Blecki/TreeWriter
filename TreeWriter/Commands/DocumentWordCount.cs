using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DocumentWordCount : ICommand
    {
        private Document Document;
        public bool Succeeded { get; private set; }

        public DocumentWordCount(Document Document)
        {
            this.Document = Document;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(Document.Contents)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
