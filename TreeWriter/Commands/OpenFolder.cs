using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class OpenFolder : ICommand
    {
        private String ProjectPath;
        public bool Succeeded { get; private set; }

        public OpenFolder(String ProjectPath)
        {
            this.ProjectPath = ProjectPath;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var project = Model.FindOpenDocument(ProjectPath) as FolderDocument;

            if (project == null)
            {
                project = new FolderDocument
                {
                    Path = ProjectPath
                };
                Model.OpenDocument(project);
            }

            try
            {
                if (project.OpenEditors.Count == 0)
                {
                    View.OpenControllerPanel(project.OpenView(Model), WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
                }
                else
                    project.OpenEditors[0].BringToFront();

                Succeeded = true;
            }
            catch (Exception e)
            {
                Model.CloseDocument(project);
                System.Windows.Forms.MessageBox.Show("Error opening folder: " + e.Message);
                Succeeded = false;
            }
        }
    }
}
