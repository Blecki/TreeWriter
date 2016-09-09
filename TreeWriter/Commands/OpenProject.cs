using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenProject : ICommand
    {
        private String ProjectPath;
        public bool Succeeded { get; private set; }

        public OpenProject(String ProjectPath)
        {
            this.ProjectPath = ProjectPath;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var project = Model.OpenProject(ProjectPath);

            if (project == null)
                return;

            if (project.OpenView == null)
            {
                project.OpenView = new DirectoryListing(project);
                View.OpenControllerPanel(project.OpenView, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            }
            else
                project.OpenView.BringToFront();

            Succeeded = true;
        }
    }
}
