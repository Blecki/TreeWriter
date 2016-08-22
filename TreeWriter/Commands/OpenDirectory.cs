using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenDirectory :ICommand
    {
        private String DirectoryPath;

        public OpenDirectory(String DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
        }

        public void Execute(ProjectModel Model, Main View)
        {
            Model.OpenDirectories.Add(DirectoryPath);
            var directoryListing = new DirectoryListing(DirectoryPath);
            View.OpenControllerPanel(directoryListing, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
        }
    }
}
