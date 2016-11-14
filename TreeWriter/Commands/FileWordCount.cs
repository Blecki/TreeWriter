using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class FileWordCount : ICommand
    {
        private String Path;
        public bool Succeeded { get; private set; }

        public FileWordCount(String Path)
        {
            this.Path = Path;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            var openDocument = Model.FindOpenDocument(Path);
            if (openDocument != null)
                System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(openDocument.GetContents())), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
            else
                System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(System.IO.File.ReadAllText(Path))), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
