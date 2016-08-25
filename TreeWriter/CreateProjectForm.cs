using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeWriterWF
{
    public partial class CreateProjectForm : Form
    {
        public String ProjectName { get { return nameBox.Text; } }
        public String ProjectPath { get { return pathBox.Text + "/" + nameBox.Text + ".twproj"; } }


        public CreateProjectForm()
        {
            InitializeComponent();
            pathBox.Text = Environment.CurrentDirectory;
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            nameBox.Focus();
            okayButton.Enabled = false;
            CancelButton = cancelButton;
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            errorLabel.Text = "";

            if (String.IsNullOrEmpty(nameBox.Text) || String.IsNullOrEmpty(pathBox.Text))
                okayButton.Enabled = false;
            else
            {
                okayButton.Enabled = true;
                if (System.IO.File.Exists(ProjectPath))
                {
                    errorLabel.Text = "Project already exists.";
                    okayButton.Enabled = false;
                }
            }            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                pathBox.Text = folderDialog.SelectedPath;
        }
    }
}
