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

namespace TreeWriterWF.Commands.Extract
{
    public partial class ManuscriptExtractor : DockablePanel
    {
        String Path;
        ExtractionSettings Settings = new ExtractionSettings();

        public ManuscriptExtractor(String Path)
        {
            this.Path = Path;

            this.InitializeComponent();

            Text = "Extract " + Path;
            extractionSettings.SelectedObject = Settings;
            Settings.SetDefaultPath(Path);
        }

        private void NoteList_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            InvokeCommand(new Commands.Extract.ExtractManuscript(Document.Data));
        }        
    }
}
