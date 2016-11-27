using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CountWords : ICommand
    {
        private String Path;
        public int Count = 0;
        public bool Succeeded { get; private set; }

        public CountWords(String Path)
        {
            this.Path = Path;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            var openCommand = new OpenPath(Path, OpenCommand.OpenStyles.Transient);
            openCommand.Execute(Model, View);
            if (openCommand.Document != null)
            {
                Count = openCommand.Document.CountWords(Model, View);
                System.Windows.Forms.MessageBox.Show(String.Format("{0} words", Count),
                    "Word count", System.Windows.Forms.MessageBoxButtons.OK);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Error", "Word count",
                    System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }
}
