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

            textEditor.Styles[1].ForeColor = Color.Red;
            textEditor.Styles[1].Hotspot = true;

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
            // Calculate folding

            int startLine = textEditor.FirstVisibleLine;
            if (startLine > 0) startLine -= 1;
            int currentFoldLevel = 1024;
            if (startLine != 0)
                currentFoldLevel = textEditor.Lines[startLine].FoldLevel;

            var endLine = textEditor.Lines.Count;
            var currentLine = startLine;

            while (currentLine != endLine)
            {
                var line = textEditor.Lines[currentLine];
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

                // Style line. 
                textEditor.StartStyling(line.Position);
                textEditor.SetStyling(line.Length, 0);

                // Search line for brackets and style between them.
                var bracketPos = line.Text.IndexOf('[');
                while (bracketPos != -1)
                {
                    var end = line.Text.IndexOf(']', bracketPos);
                    if (end != -1)
                    {
                        textEditor.StartStyling(line.Position + bracketPos);
                        textEditor.SetStyling(end - bracketPos + 1, 1);

                        bracketPos = line.Text.IndexOf('[', end);
                    }
                    else
                        bracketPos = -1;
                }

                currentLine += 1;
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

        private void textEditor_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            var endPoint = textEditor.Text.IndexOf(']', e.Position);
            var startPoint = textEditor.Text.LastIndexOf('[', e.Position, e.Position + 1);
            var linkText = textEditor.Text.Substring(startPoint + 1, endPoint - startPoint - 1);
            ControllerCommand(new Commands.WikiFollow(Document, linkText));
        }               
    }
}
