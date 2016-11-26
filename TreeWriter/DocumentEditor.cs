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
using ScintillaNET;

namespace TreeWriterWF
{
    public partial class DocumentEditor : DockablePanel
    {
        protected EditableDocument Document;

        public DocumentEditor()
        {
            InitializeComponent();
        }
        
        public DocumentEditor(EditableDocument Document)
        {
            this.Document = Document;

            InitializeComponent();

            Text = Document.GetEditorTitle();
        }

        public override void ReloadDocument()
        {
            Text = Document.GetEditorTitle();
        }

        private void DocumentEditor_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if (CloseStyle == EditableDocument.CloseStyle.Natural && e.CloseReason == CloseReason.UserClosing)
            { 
                var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
                InvokeCommand(closeCommand);
                e.Cancel = closeCommand.Cancel;
            }
        }
  
        private void DocumentEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                InvokeCommand(new Commands.SaveDocument(Document));
            }
        }

        private void duplicateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.DuplicateView(Document));
        }

        private void wordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", Document.CountWords()), "Word count",
                System.Windows.Forms.MessageBoxButtons.OK);
        }

        private void saveDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.SaveDocument(Document));
        }

        private void closeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); // Close event takes care of the saving and cancelling.
        }       
    }
}
