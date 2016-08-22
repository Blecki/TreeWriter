using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DeleteFile :ICommand
    {
        private String Path;

        public DeleteFile(String Path)
        {
            this.Path = Path;
        }

        public void Execute(ProjectModel Model, Main View)
        {
            try
            {
                System.IO.File.Delete(Path);
            }
            catch (System.IO.IOException e)
            {
                System.Windows.Forms.MessageBox.Show("Operation failed.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }
}
