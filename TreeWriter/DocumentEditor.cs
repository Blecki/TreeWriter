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

        public DocumentEditor(Document Document, ScintillaNET.Document? LinkingDocument)
        {
            this.Document = Document;

            InitializeComponent();

            #region Setup Folding Margins

            textEditor.Lexer = Lexer.Container;

            textEditor.SetProperty("fold", "1");
            textEditor.SetProperty("fold.compact", "1");
            
            // Configure a margin to display folding symbols
            textEditor.Margins[2].Type = MarginType.Symbol;
            textEditor.Margins[2].Mask = Marker.MaskFolders;
            textEditor.Margins[2].Sensitive = true;
            textEditor.Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                textEditor.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                textEditor.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            textEditor.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            textEditor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            textEditor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            textEditor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            textEditor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            textEditor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            textEditor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            textEditor.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            #endregion

            // Load document into editor.
            if (!LinkingDocument.HasValue)
                textEditor.Text = Document.Contents;
            else
                textEditor.Document = LinkingDocument.Value;

            UpdateTitle();

            //Register last to avoid spurius events
            this.textEditor.TextChanged += new System.EventHandler(this.textEditor_TextChanged);

        }

        public void UpdateTitle()
        {
            Text = System.IO.Path.GetFileNameWithoutExtension(Document.FileName) + (Document.NeedChangesSaved ? "*" : "");
        }

        private void textEditor_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            int startLine = textEditor.FirstVisibleLine;
            if (startLine > 0) startLine -= 1;
            int currentFoldLevel = 1024;
            if (startLine != 0)
                currentFoldLevel = textEditor.Lines[startLine].FoldLevel;

            var endLine = textEditor.Lines.Count;

            while (startLine != endLine)
            {
                var line = textEditor.Lines[startLine];
                var headerSize = CountHashes(line.Text);

                if (headerSize > 0)
                {
                    line.FoldLevelFlags = FoldLevelFlags.Header;
                    line.FoldLevel = 1024 + headerSize - 1;

                    currentFoldLevel = 1024 + headerSize;
                }
                else
                {
                    line.FoldLevelFlags = 0;
                    line.FoldLevel = currentFoldLevel;
                }

                startLine += 1;
            }
        }

        private int CountHashes(String Line)
        {
            int count = 0;
            while (count < Line.Length && Line[count] == '#') ++count;
            return count;
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            Document.ApplyChanges(textEditor.Text);
        }

        private void DocumentEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
            ControllerCommand(closeCommand);
            e.Cancel = closeCommand.Cancel;
        }               
    }
}
