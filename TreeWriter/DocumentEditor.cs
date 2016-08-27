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
        String WordBoundaries = " \t\r\n.,;:\\/\"?![]{}()<>#-";

        public DocumentEditor(Document Document, ScintillaNET.Document? LinkingDocument, NHunspell.Hunspell SpellChecker)
        {
            this.Document = Document;
            this.SpellChecker = SpellChecker;

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

            textEditor.Styles[1].ForeColor = Color.Blue;
            textEditor.Styles[1].Hotspot = true;

            textEditor.Indicators[1].Style = IndicatorStyle.Squiggle;
            textEditor.Indicators[1].ForeColor = Color.Red;


            // Load document into editor.
            if (!LinkingDocument.HasValue)
                textEditor.Text = Document.Contents;
            else
                textEditor.Document = LinkingDocument.Value;

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

        private void textEditor_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            // Calculate folding

            int startLine = textEditor.LineFromPosition(textEditor.CharPositionFromPoint(0, 0));
            if (startLine > 0) startLine -= 1;
            int currentFoldLevel = 1048;
            if (startLine != 0)
                currentFoldLevel = textEditor.Lines[startLine].FoldLevel;

            var endLine = textEditor.Lines.Count;
            var currentLine = startLine;

            while (currentLine < endLine)
            {
                var line = textEditor.Lines[currentLine];
                var headerSize = CountHashes(line.Text);

                if (headerSize > 0)
                {
                    line.FoldLevelFlags = FoldLevelFlags.Header;
                    line.FoldLevel = 1048 - headerSize - 1;
                    currentFoldLevel = 1048 - headerSize;
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

                // Finally, check for spelling.

                // Clear any existing squiggles.
                textEditor.IndicatorCurrent = 1;
                textEditor.IndicatorClearRange(line.Position, line.EndPosition - line.Position);

                // Find all words in line.
                var startPos = FindWordStart(line.Text, 0);
                while(startPos < line.Length)
                {
                    var end = FindWordEnd(line.Text, startPos);
                    // Style between startPos and end
                    var word = line.Text.Substring(startPos, end - startPos);
                    var spellingResults = SpellChecker.Spell(word);
                    if (!spellingResults)
                        textEditor.IndicatorFillRange(line.Position + startPos, end - startPos);

                    startPos = FindWordStart(line.Text, end);
                }

                currentLine += 1;
            }

            
        }

        private int FindWordStart(String Str, int Start)
        {
            while (Start < Str.Length && WordBoundaries.Contains(Str[Start]))
                Start += 1;
            return Start;
        }

        private int FindWordEnd(String Str, int Start)
        {
            while (Start < Str.Length && !WordBoundaries.Contains(Str[Start]))
                Start += 1;
            return Start;
        }

        private int FindWordStartBackwards(String Str, int Start)
        {
            while (Start >= 0 && !WordBoundaries.Contains(Str[Start]))
                Start -= 1;
            return Start + 1;
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
            if (e.CloseReason == CloseReason.UserClosing)
            { 
            var closeCommand = new Commands.CloseEditor(Document, this, e.CloseReason == CloseReason.MdiFormClosing);
            ControllerCommand(closeCommand);
            e.Cancel = closeCommand.Cancel;
                }
        }

        private void textEditor_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            var endPoint = textEditor.Text.IndexOf(']', e.Position);
            var startPoint = textEditor.Text.LastIndexOf('[', e.Position, e.Position + 1);
            var linkText = textEditor.Text.Substring(startPoint + 1, endPoint - startPoint - 1);
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
            //Open a new document editor.
            ControllerCommand(new Commands.DuplicateView(Document));
        }

        private void textEditor_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void textEditor_IndicatorClick(object sender, IndicatorClickEventArgs e)
        {
            var style = textEditor.IndicatorAllOnFor(e.Position);
            if ((style & 2) == 0) return;

            var lineIndex = textEditor.LineFromPosition(e.Position);
            var line = textEditor.Lines[lineIndex];
            var offset = e.Position - line.Position;
            var wordStart = FindWordStartBackwards(line.Text, offset);
            var wordEnd = FindWordEnd(line.Text, offset);
            var word = line.Text.Substring(wordStart, wordEnd - wordStart);

            var suggestions = SpellChecker.Suggest(word);

            var contextMenu = new ContextMenuStrip();
            if (suggestions.Count == 0)
                contextMenu.Items.Add("No suggestions");
            else
            {
                foreach (var suggestion in suggestions)
                {
                    var item = new ToolStripMenuItem(suggestion);
                    item.Click += (s, args) =>
                    {
                        textEditor.TargetStart = wordStart + line.Position;
                        textEditor.TargetEnd = wordEnd + line.Position;
                        textEditor.ReplaceTarget(item.Text);
                    };
                    contextMenu.Items.Add(item);
                }
            }
            var addOption = new ToolStripMenuItem("add to dictionary");
            addOption.Click += (s, args) =>
                {
                    ControllerCommand(new Commands.AddWordToDictionary(word));
                    textEditor.Invalidate();
                };
            contextMenu.Items.Add(addOption);


            contextMenu.Show(this, textEditor.PointToClient(Control.MousePosition));
        }
       
    }
}
