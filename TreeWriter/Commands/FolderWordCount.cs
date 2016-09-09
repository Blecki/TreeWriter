using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class FolderWordCount : ICommand
    {
        private String Path;
        public bool Succeeded { get; private set; }

        public FolderWordCount(String Path)
        {
            this.Path = Path;
            Succeeded = true;
        }

        private int TotalDirectory(String Path, Model Model)
        {
            var total = 0;
            foreach (var directory in System.IO.Directory.EnumerateDirectories(Path))
                total += TotalDirectory(directory, Model);
            foreach (var file in System.IO.Directory.EnumerateFiles(Path))
                total += TotalDocument(file, Model);
            return total;
        }

        private int TotalDocument(String Path, Model Model)
        {
            var openDocument = Model.FindOpenDocument(Path);
            if (openDocument != null)
                return WordParser.CountWords(openDocument.Contents);
            else
                return WordParser.CountWords(System.IO.File.ReadAllText(Path));
        }

        public void Execute(Model Model, Main View)
        {
            var totalWordCount = TotalDirectory(Path, Model);
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", totalWordCount), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
