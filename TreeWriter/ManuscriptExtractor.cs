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
    public partial class ManuscriptExtractor : DockablePanel
    {
        ManuscriptDocument Document;
        Commands.ExtractionSettings Settings;

        public ManuscriptExtractor(ManuscriptDocument Document)
        {
            this.Document = Document;
            this.Settings = new Commands.ExtractionSettings(Document);

            this.InitializeComponent();

            Text = Document.GetEditorTitle();
            extractionSettings.SelectedObject = Settings;
        }

        private void NoteList_FormClosing(object sender, FormClosingEventArgs e)
        {
            Document.OpenEditors.Remove(this);
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.ExtractManuscript(Document.Data, Settings));
        }        
    }
}
