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
    public partial class TextDocumentEditor : DocumentEditor
    {
        NHunspell.Hunspell SpellChecker;
        NHunspell.MyThes Thesaurus;

        public TextDocumentEditor(
            TextDocument Document, 
            ScintillaNET.Document? LinkingDocument, 
            NHunspell.Hunspell SpellChecker,
            NHunspell.MyThes Thesaurus) : base(Document)
        {
            this.SpellChecker = SpellChecker;
            this.Thesaurus = Thesaurus;

            this.InitializeComponent();
            
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
            base.ReloadDocument();
        }

        public ScintillaNET.Document GetScintillaDocument()
        {
            return textEditor.Document;
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            Document.ApplyChanges(textEditor.Text);
        }
          
        private void DocumentEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && e.Control)
            {
                //Send to scrap file.

                var text = textEditor.SelectedText;
                if (!String.IsNullOrEmpty(text))
                {
                    InvokeCommand(new Commands.SendToScrap(text, Document.Path));
                    textEditor.ReplaceSelection("");
                }
            }
        }       
    }
}
