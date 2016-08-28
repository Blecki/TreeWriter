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

        public DocumentWordCount(Document Document)
        {
            this.Document = Document;
        }

        public void Execute(Model Model, Main View)
        {
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(Document.Contents)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
