using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenProject :ICommand
    {
        private String ProjectPath;

        public OpenProject(String ProjectPath)
        {
            this.ProjectPath = ProjectPath;
        }

        public void Execute(Model Model, Main View)
        {
            var project = Model.OpenProject(ProjectPath);
            if (project.OpenView == null)
            {
                project.OpenView = new DirectoryListing(project);
                View.OpenControllerPanel(project.OpenView, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            }
            else
                project.OpenView.BringToFront();
        }
    }
}
