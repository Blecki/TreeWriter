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
        private bool ShowPopup = true;
        public int Count = 0;
        public bool Succeeded { get; private set; }

        public FileWordCount(String Path, bool ShowPopup = true)
        {
            this.Path = Path;
            this.ShowPopup = ShowPopup;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            var openCommand = new OpenPath(Path, OpenCommand.OpenStyles.Transient);
            openCommand.Execute(Model, View);
            if (openCommand.Document != null)
            {
                Count = openCommand.Document.CountWords();
                if (ShowPopup)
                    System.Windows.Forms.MessageBox.Show(String.Format("{0} words", Count),
                        "Word count", System.Windows.Forms.MessageBoxButtons.OK);
            }
            else
            {
                if (ShowPopup)
                    System.Windows.Forms.MessageBox.Show("Error", "Word count",
                        System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }
}
