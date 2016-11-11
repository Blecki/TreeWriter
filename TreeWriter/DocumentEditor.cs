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
    public partial class DocumentEditor : ControllerPanel
    {
        Document Document;
        NHunspell.Hunspell SpellChecker;
        NHunspell.MyThes Thesaurus;

        public DocumentEditor(Document Document, ScintillaNET.Document? LinkingDocument, 
            NHunspell.Hunspell SpellChecker,
            NHunspell.MyThes Thesaurus)
        {
            this.Document = Document;
            this.SpellChecker = SpellChecker;
            this.Thesaurus = Thesaurus;

            InitializeComponent();
            
            // Load document into editor.
            if (!LinkingDocument.HasValue)
                textEditor.Text = Document.Contents;
            else
                textEditor.Document = LinkingDocument.Value;

            textEditor.Create(SpellChecker, Thesaurus, (a) => ControllerCommand(a));

            UpdateTitle();

            //Register last to avoid spurius events
            this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);
        }

        public ScintillaNET.Document GetScintillaDocument()
        {
            return textEditor.Document;
        }

        public void UpdateTitle()
        {
            Text = System.IO.Path.GetFileNameWithoutExtension(Document.FileName) + (Document.NeedChangesSaved ? "*" : "");
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            Document.ApplyChanges(textEditor.Text);
        }

        private void DocumentEditor_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if (e.CloseReason == CloseReason.UserClosing)
            { 
            var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
            ControllerCommand(closeCommand);
            e.Cancel = closeCommand.Cancel;
                }
        }

        private void textEditor_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            var linkText = textEditor.GetWordFromPosition(e.Position);
            ControllerCommand(new Commands.FollowWikiLink(Document, linkText));
        }
          
        private void DocumentEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ControllerCommand(new Commands.SaveDocument(Document));
            }
        }

        private void duplicateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControllerCommand(new Commands.DuplicateView(Document));
        }

        private void wordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(Document.Contents)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }       
    }
}
