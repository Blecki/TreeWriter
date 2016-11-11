using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScintillaNET;

namespace TreeWriterWF
{
    public class TextEditor : ScintillaNET.Scintilla
    {
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

        Action<ICommand> ControllerCommand;

        public void Create(NHunspell.Hunspell SpellChecker, NHunspell.MyThes Thesaurus, Action<ICommand> ControllerCommand)
        {
            this.SpellChecker = SpellChecker;
            this.Thesaurus = Thesaurus;
            this.ControllerCommand = ControllerCommand;
        }

        public TextEditor()
        {
            this.AnnotationVisible = ScintillaNET.Annotation.Boxed;
            this.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click)
            | ScintillaNET.AutomaticFold.Change)));
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            //this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IdleStyling = ScintillaNET.IdleStyling.ToVisible;
            this.WrapMode = ScintillaNET.WrapMode.Word;
            this.Zoom = 5;
            if (!this.DesignMode) this.StyleNeeded += new System.EventHandler<ScintillaNET.StyleNeededEventArgs>(this.textEditor_StyleNeeded);
            if (!this.DesignMode) this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textEditor_MouseDown);

            #region Setup Folding Margins

            Lexer = Lexer.Container;

            SetProperty("fold", "1");
            SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            Margins[2].Type = MarginType.Symbol;
            Margins[2].Mask = Marker.MaskFolders;
            Margins[2].Sensitive = true;
            Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                Markers[i].SetForeColor(SystemColors.ControlLightLight);
                Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            #endregion

            StyleClearAll();

            Styles[1].ForeColor = Color.Blue;
            Styles[1].Hotspot = true;

            Styles[2].ForeColor = Color.Green;
            Styles[2].Italic = true;

            Styles[3].Italic = true;

            Indicators[1].Style = IndicatorStyle.Squiggle;
            Indicators[1].ForeColor = Color.Red;

            if (!this.DesignMode) initContextMenu();
        }
                
        private void initContextMenu()
        {
            this.miUndo = new MenuItem("Undo", (s, ea) => Undo());
            this.miRedo = new MenuItem("Redo", (s, ea) => Redo());
            this.miCut = new MenuItem("Cut", (s, ea) => Cut());
            this.miCopy = new MenuItem("Copy", (s, ea) => Copy());
            this.miPaste = new MenuItem("Paste", (s, ea) => Paste());
            this.miDelete = new MenuItem("Delete", (s, ea) => ReplaceSelection(""));
            this.miSelectAll = new MenuItem("Select All", (s, ea) => SelectAll());
            this.miDefineWord = new MenuItem("Define word");
            this.miDefineWord.Click += (sender, args) =>
                {
                    MessageBox.Show(WordAtPoint(ContextPoint));
                };
            this.miThesarus = new MenuItem("Thesarus");
            this.miThesarus.Click += (sender, args) =>
                {
                    var charPosition = CharPositionFromPointClose(ContextPoint.X, ContextPoint.Y);
                    if (charPosition == -1) return;

                    var lineIndex = LineFromPosition(charPosition);
                    var line = Lines[lineIndex];
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
                                TargetStart = wordStart + line.Position;
                                TargetEnd = wordEnd + line.Position;
                                ReplaceTarget(item.Text);
                            };
                            contextMenu.Items.Add(item);
                        }
                        contextMenu.Show(this, PointToClient(Control.MousePosition));
                    }
                };

            this.miSelectionWordCount = new MenuItem("selection word count");
            this.miSelectionWordCount.Click += (sender, args) =>
                {
                    System.Windows.Forms.MessageBox.Show(String.Format("{0} words", WordParser.CountWords(SelectedText)), "Word count", System.Windows.Forms.MessageBoxButtons.OK);
                };
        }

        private void textEditor_StyleNeeded(object sender, StyleNeededEventArgs e)
        {
            // Calculate folding

            int startLine = LineFromPosition(CharPositionFromPoint(0, 0));
            if (startLine > 0) startLine -= 1;
            int currentFoldLevel = 1048;
            if (startLine != 0)
                currentFoldLevel = Lines[startLine].FoldLevel;

            var endLine = startLine + 25;
            var currentLine = startLine;

            while (currentLine < endLine && currentLine < Lines.Count)
            {
                var line = Lines[currentLine];
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
                StartStyling(line.Position);
                SetStyling(line.Length, 0);

                var bracketPos = line.Text.IndexOf('*');
                while (bracketPos != -1)
                {
                    var end = line.Text.IndexOf('*', bracketPos + 1);
                    if (end != -1)
                    {
                        StartStyling(line.Position + bracketPos);
                        SetStyling(end - bracketPos + 1, 3);

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
                        StartStyling(line.Position + bracketPos);
                        SetStyling(end - bracketPos + 1, 2);

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
                        StartStyling(line.Position + bracketPos);
                        SetStyling(end - bracketPos + 1, 1);

                        bracketPos = line.Text.IndexOf('<', end);
                    }
                    else
                        bracketPos = -1;
                }

                // Finally, check for spelling.

                // Clear any existing squiggles.
                IndicatorCurrent = 1;
                IndicatorClearRange(line.Position, line.EndPosition - line.Position);

                if (SpellChecker != null)
                {
                    // Find all words in line.
                    var startPos = FindWordStart(line.Text, 0);
                    while (startPos < line.Length)
                    {
                        var end = FindWordEnd(line.Text, startPos);
                        // Style between startPos and end
                        var word = line.Text.Substring(startPos, end - startPos);
                        var spellingResults = SpellChecker.Spell(word);
                        if (!spellingResults)
                            IndicatorFillRange(line.Position + startPos, end - startPos);

                        startPos = FindWordStart(line.Text, end);
                    }
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
            var pos = CharPositionFromPointClose(p.X, p.Y);
            if (pos == -1) return null;
            return GetWordFromPosition(pos);
        }

        private String WordAtCharPosition(int p)
        {
            var lineIndex = LineFromPosition(p);
            var line = Lines[lineIndex];
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
                        
        private void textEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                miUndo.Enabled = CanUndo;
                miRedo.Enabled = CanRedo;
                miCut.Enabled = true;
                miCopy.Enabled = true;
                miDelete.Enabled = Text.Length > 0;
                miSelectAll.Enabled = true;
                ContextPoint = e.Location;

                this.ContextMenu = new ContextMenu();

                var charPosition = CharPositionFromPointClose(ContextPoint.X, ContextPoint.Y);
                if (SpellChecker != null && charPosition != -1 && ((IndicatorAllOnFor(charPosition) & 2) == 2))
                {
                    var lineIndex = LineFromPosition(charPosition);
                    var line = Lines[lineIndex];
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
                                TargetStart = wordStart + line.Position;
                                TargetEnd = wordEnd + line.Position;
                                ReplaceTarget(item.Text);
                            };
                            this.ContextMenu.MenuItems.Add(item);
                        }
                    }
                    var addOption = new MenuItem("add to dictionary");
                    addOption.Click += (s, args) =>
                        {
                            ControllerCommand(new Commands.AddWordToDictionary(word));
                            Invalidate();
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
    }
}
