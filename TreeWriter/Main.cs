using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TreeWriterWF
{
    public partial class Main : Form
    {
        private Model ProjectModel = new Model();

        public Main()
        {
            InitializeComponent();

            ProjectModel.LoadSettings(this);
        }

        public void OpenControllerPanel(ControllerPanel Panel, DockState Where)
        {
            Panel.ControllerCommand += ProcessControllerCommand;
            Panel.Show(dockPanel, Where);
        }

        internal void ProcessControllerCommand(ICommand Command)
        {
            // TODO: Queue these up or something?
            // TODO: Implement undo
            Command.Execute(ProjectModel, this);
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.Filter = "Tree Writer Projects|*.twproj";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ProcessControllerCommand(new Commands.OpenProject(fileDialog.FileName));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProjectModel.SaveSettings();
        }

        private void createProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var projectDialog = new CreateProjectForm();
            if (projectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ProcessControllerCommand(new Commands.CreateProject(projectDialog.ProjectPath));
        }
    }
}
