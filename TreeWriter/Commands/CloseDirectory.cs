using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CloseDirectory : ICommand
    {
        String DirectoryPath;

        public CloseDirectory(String DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
        }

        public void Execute(ProjectModel Model, Main View)
        {
            Model.OpenDirectories.Remove(DirectoryPath);
        }
    }
}
