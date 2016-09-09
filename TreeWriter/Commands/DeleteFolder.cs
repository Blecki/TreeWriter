using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DeleteFolder : ICommand
    {
        private String DirectoryPath;
        public bool Succeeded { get; private set; }
        
        public DeleteFolder(String DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                System.IO.Directory.Delete(DirectoryPath, true);
                Succeeded = !System.IO.Directory.Exists(DirectoryPath);
            }
            catch (System.IO.IOException e)
            {
                System.Windows.Forms.MessageBox.Show("Operation failed.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }
}
