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
    public partial class TextDocumentEditor : DockablePanel
    {
        TextDocument Document;
        NHunspell.Hunspell SpellChecker;
        NHunspell.MyThes Thesaurus;

        public TextDocumentEditor(
            TextDocument Document, 
            ScintillaNET.Document? LinkingDocument, 
            NHunspell.Hunspell SpellChecker,
            NHunspell.MyThes Thesaurus)
        {
            this.Document = Document;
            this.SpellChecker = SpellChecker;
            this.Thesaurus = Thesaurus;

            InitializeComponent();
            
            // Load document into editor.
            if (!LinkingDocument.HasValue)
                textEditor.Text = Document.GetContents();
            else
                textEditor.Document = LinkingDocument.Value;

            textEditor.Create(SpellChecker, Thesaurus, (a) => InvokeCommand(a));

            Text = Document.GetEditorTitle();

            //Register last to avoid spurius events
            this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);
        }

        public override void ReloadDocument()
        {
            textEditor.Text = Document.GetContents();
            Text = Document.GetEditorTitle();
        }

        public ScintillaNET.Document GetScintillaDocument()
        {
            return textEditor.Document;
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            Document.ApplyChanges(textEditor.Text);
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

        private void textEditor_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            //var linkText = textEditor.GetWordFromPosition(e.Position);
            //ControllerCommand(new Commands.FollowWikiLink(Document, linkText));
        }
          
        private void DocumentEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                InvokeCommand(new Commands.SaveDocument(Document));
            }
            else if (e.KeyCode == Keys.D && e.Control)
            {
                //Send to scrap file.

                var text = textEditor.SelectedText;
                InvokeCommand(new Commands.SendToScrap(text, Document.Path));
                textEditor.ReplaceSelection("");
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
    }
}
