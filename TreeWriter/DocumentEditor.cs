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
        String WordBoundaries = " \t\r\n.,;:\\/\"?![]{}()<>#-'`";
        Point ContextPoint;

        #region Custom Context Menu Items
        MenuItem miUndo;
        MenuItem miRedo;
        MenuItem miCut;
        MenuItem miCopy;
        MenuItem miDelete;
        MenuItem miSelectAll;
        MenuItem miPaste;
        MenuItem miDefineWord;
        MenuItem miThesarus;
        MenuItem miSelectionWordCount;
        #endregion

        public DocumentEditor(Document Document, ScintillaNET.Document? LinkingDocument, 
            NHunspell.Hunspell SpellChecker,
            NHunspell.MyThes Thesaurus)
        {
            this.Document = Document;
            this.SpellChecker = SpellChecker;
            this.Thesaurus = Thesaurus;

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

            textEditor.StyleClearAll();

            textEditor.Styles[1].ForeColor = Color.Blue;
            textEditor.Styles[1].Hotspot = true;

            textEditor.Styles[2].ForeColor = Color.Green;
            textEditor.Styles[2].Italic = true;

            textEditor.Styles[3].Italic = true;

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

            initContextMenu();

        }

        public ScintillaNET.Document GetScintillaDocument()
        {
            return textEditor.Document;
        }

        private void initContextMenu()
        {
            this.miUndo = new MenuItem("Undo", (s, ea) => textEditor.Undo());
            this.miRedo = new MenuItem("Redo", (s, ea) => textEditor.Redo());
            this.miCut = new MenuItem("Cut", (s, ea) => textEditor.Cut());
            this.miCopy = new MenuItem("Copy", (s, ea) => textEditor.Copy());
            this.miPaste = new MenuItem("Paste", (s, ea) => textEditor.Paste());
            this.miDelete = new MenuItem("Delete", (s, ea) => textEditor.ReplaceSelection(""));
            this.miSelectAll = new MenuItem("Select All", (s, ea) => textEditor.SelectAll());
            this.miDefineWord = new MenuItem("Define word");
            this.miDefineWord.Click += (sender, args) =>
                {
                    MessageBox.Show(WordAtPoint(ContextPoint));
                };
            this.miThesarus = new MenuItem("Thesarus");
            this.miThesarus.Click += (sender, args) =>
                {
                    var charPosition = textEditor.CharPositionFromPointClose(ContextPoint.X, ContextPoint.Y);
                    if (charPosition == -1) return;

                    var lineIndex = textEditor.LineFromPosition(charPosition);
                    var line = textEditor.Lines[lineIndex];
                    var offset = charPosition - line.Position;
                    var wordStart = FindWordStartBackwards(line.Text, offset);
                    var wordEnd = FindWordEnd(line.Text, offset);
                    if (wordEnd <= wordStart) return;
                    var word = line.Text.Substring(wordStart, wordEnd - wordStart);

                    var suggestions = Thesaurus.Lookup(word);

                    if (suggestions != null)
                    {
                        var contextMenu = new ContextMenuStrip();
                        foreach (var meaning in suggestions.GetSynonyms())
                        {
                            var item = new ToolStripMenuItem(meaning.Key);
                            item.Click += (s, _a) =>
                            {
                                textEditor.TargetStart = wordStart + line.Position;
                                textEditor.TargetEnd = wordEnd + line.Position;
                                textEditor.ReplaceTarget(item.Text);
                            };
                            contextMenu.Items.Add(item);
                        }
                        contextMenu.Show(this, textEditor.PointToClient(Control.MousePosition));
                    }
                };

            this.miSelectionWordCount = new MenuItem("selection word count");
            this.miSelectionWordCount.Click += (sender, args) =>
                {
                    System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(textEditor.SelectedText)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
                };
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

            var endLine = startLine + 25;
            var currentLine = startLine;

            statusLabel.Text = String.Format("Styling: {0} to {1}", startLine, endLine);

            while (currentLine < endLine && currentLine < textEditor.Lines.Count)
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

                var bracketPos = line.Text.IndexOf('*');
                while (bracketPos != -1)
                {
                    var end = line.Text.IndexOf('*', bracketPos + 1);
                    if (end != -1)
                    {
                        textEditor.StartStyling(line.Position + bracketPos);
                        textEditor.SetStyling(end - bracketPos + 1, 3);

                        bracketPos = line.Text.IndexOf('*', end + 1);
                    }
                    else
                        bracketPos = -1;
                }

                // Search line for brackets and style between them.
                bracketPos = line.Text.IndexOf('[');
                while (bracketPos != -1)
                {
                    var end = line.Text.IndexOf(']', bracketPos);
                    if (end != -1)
                    {
                        textEditor.StartStyling(line.Position + bracketPos);
                        textEditor.SetStyling(end - bracketPos + 1, 2);

                        bracketPos = line.Text.IndexOf('[', end);
                    }
                    else
                        bracketPos = -1;
                }

                // Search line for brackets and style between them.
                bracketPos = line.Text.IndexOf('<');
                while (bracketPos != -1)
                {
                    var end = line.Text.IndexOf('>', bracketPos);
                    if (end != -1)
                    {
                        textEditor.StartStyling(line.Position + bracketPos);
                        textEditor.SetStyling(end - bracketPos + 1, 1);

                        bracketPos = line.Text.IndexOf('<', end);
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

        private String WordAtPoint(Point p)
        {
            var pos = textEditor.CharPositionFromPointClose(p.X, p.Y);
            if (pos == -1) return null;
            return textEditor.GetWordFromPosition(pos);
        }

        private String WordAtCharPosition(int p)
        {
            var lineIndex = textEditor.LineFromPosition(p);
            var line = textEditor.Lines[lineIndex];
            var offset = p - line.Position;
            var wordStart = FindWordStartBackwards(line.Text, offset);
            var wordEnd = FindWordEnd(line.Text, offset);
            var word = line.Text.Substring(wordStart, wordEnd - wordStart);
            return word;
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

        private void textEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                miUndo.Enabled = textEditor.CanUndo;
                miRedo.Enabled = textEditor.CanRedo;
                miCut.Enabled = true;
                miCopy.Enabled = true;
                miDelete.Enabled = textEditor.Text.Length > 0;
                miSelectAll.Enabled = true;
                ContextPoint = e.Location;

                this.ContextMenu = new ContextMenu();

                var charPosition = textEditor.CharPositionFromPointClose(ContextPoint.X, ContextPoint.Y);
                if (charPosition != -1 && ((textEditor.IndicatorAllOnFor(charPosition) & 2) == 2))
                {
                    var lineIndex = textEditor.LineFromPosition(charPosition);
                    var line = textEditor.Lines[lineIndex];
                    var offset = charPosition - line.Position;
                    var wordStart = FindWordStartBackwards(line.Text, offset);
                    var wordEnd = FindWordEnd(line.Text, offset);
                    var word = line.Text.Substring(wordStart, wordEnd - wordStart);

                    var suggestions = SpellChecker.Suggest(word);

                    if (suggestions == null || suggestions.Count == 0)
                        this.ContextMenu.MenuItems.Add("No suggestions");
                    else
                    {
                        foreach (var suggestion in suggestions)
                        {
                            var item = new MenuItem(suggestion);
                            item.Click += (s, args) =>
                            {
                                textEditor.TargetStart = wordStart + line.Position;
                                textEditor.TargetEnd = wordEnd + line.Position;
                                textEditor.ReplaceTarget(item.Text);
                            };
                            this.ContextMenu.MenuItems.Add(item);
                        }
                    }
                    var addOption = new MenuItem("add to dictionary");
                    addOption.Click += (s, args) =>
                        {
                            ControllerCommand(new Commands.AddWordToDictionary(word));
                            textEditor.Invalidate();
                        };
                    this.ContextMenu.MenuItems.Add(addOption);
                    this.ContextMenu.MenuItems.Add(new MenuItem("-"));
                }

                this.ContextMenu.MenuItems.Add(this.miUndo);
                this.ContextMenu.MenuItems.Add(this.miRedo);
                this.ContextMenu.MenuItems.Add(new MenuItem("-"));
                this.ContextMenu.MenuItems.Add(miCut);
                this.ContextMenu.MenuItems.Add(miCopy);
                this.ContextMenu.MenuItems.Add(miPaste);
                this.ContextMenu.MenuItems.Add(miDelete);
                this.ContextMenu.MenuItems.Add(new MenuItem("-"));
                this.ContextMenu.MenuItems.Add(miSelectAll);
                this.ContextMenu.MenuItems.Add(miSelectionWordCount);
                this.ContextMenu.MenuItems.Add(new MenuItem("-"));
                this.ContextMenu.MenuItems.Add(miDefineWord);
                this.ContextMenu.MenuItems.Add(miThesarus);
                this.ContextMenu.Show(this, e.Location);
            }
        }

        private void wordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(Document.Contents)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
        }       
    }
}
