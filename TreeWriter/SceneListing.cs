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
    public partial class SceneListing : ControllerPanel
    {
        private ManuscriptDocument Document;
        private int ContextNode;

        public SceneListing(ManuscriptDocument Document)
        {
            this.Document = Document;
           
            InitializeComponent();
        }

        private void UpdateList()
        {
            listBox.Items.Clear();
            listBox.Items.AddRange(Document.Scenes.Select(s => s.Name).ToArray());
            listBox.Invalidate();
        }

        private void _FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
                ControllerCommand(closeCommand);
                e.Cancel = closeCommand.Cancel;
            }
        }

        private void listBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextNode = listBox.IndexFromPoint(e.Location);
                contextMenu.Show(this, e.Location);
            }
        }

        private bool Confirm(String Text)
        {
            var r = MessageBox.Show(Text, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return r == System.Windows.Forms.DialogResult.Yes;
        }
    }
}
