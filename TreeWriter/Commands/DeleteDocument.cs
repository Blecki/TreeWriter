using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class DeleteDocument : ICommand
    {
        private String Path;
        public bool Succeeded { get; private set; }

        public DeleteDocument(String Path)
        {
            this.Path = Path;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            try
            {
                var openDocument = Model.FindOpenDocument(Path);
                if (openDocument != null)
                    System.Windows.Forms.MessageBox.Show("Close this document before deleting it.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
                else
                {
                    System.IO.File.Delete(Path);
                    Succeeded = !System.IO.File.Exists(Path);
                }
            }
            catch (System.IO.IOException e)
            {
                System.Windows.Forms.MessageBox.Show("Operation failed.", "Alert!", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }
}
