using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CreateProject : ICommand
    {
        private String ProjectPath;

        public CreateProject(String ProjectPath)
        {
            this.ProjectPath = ProjectPath;
        }

        public void Execute(Model Model, Main View)
        {
            if (System.IO.File.Exists(ProjectPath))
            {
                System.Windows.Forms.MessageBox.Show("Project already exists.");
                return;
            }

            System.IO.File.WriteAllText(ProjectPath, "TREE WRITER PROJECT");

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
