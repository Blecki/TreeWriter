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

        private int TotalDirectory(String Path, Model Model, Main View)
        {
            // TODO: Don't count scrap.txt

            var total = 0;
            foreach (var directory in System.IO.Directory.EnumerateDirectories(Path))
                total += TotalDirectory(directory, Model, View);
            foreach (var file in System.IO.Directory.EnumerateFiles(Path))
                total += TotalDocument(file, Model, View);
            return total;
        }

        private int TotalDocument(String Path, Model Model, Main View)
        {
            var countCommand = new FileWordCount(Path, false);
            countCommand.Execute(Model, View);
            return countCommand.Count;
        }

        public void Execute(Model Model, Main View)
        {
            var totalWordCount = TotalDirectory(Path, Model, View);
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", totalWordCount), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }
    }
}
