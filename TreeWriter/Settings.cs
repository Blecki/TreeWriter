using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;

namespace TreeWriterWF
{
    public class HiliteWord
    {
        private String _word;
        public String Word { get { return _word; } set { _word = value.ToUpper(); } }
        public int Style { get; set; }
    }

    public class Style
    {
        public System.Drawing.Color ForeColor {get; set;}
        public System.Drawing.Color BackColor { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        public Style()
        {
            ForeColor = System.Drawing.Color.Black;
            BackColor = System.Drawing.Color.White;
            Bold = false;
            Italic = false;
        }
    }

    public class Settings
    {
        [Newtonsoft.Json.JsonIgnore]
        public Action OnSettingsChanged;
        public List<String> RecentDocuments;

        [Newtonsoft.Json.JsonIgnore]
        public static Settings GlobalSettings = null;

        [Description("Base name of dictionary files to use. Requires restart to apply.")]
        [Category("Spellchecker")]
        public String Dictionary { get; set; }

        [Description("Thesarus data file. Requires restart to apply.")]
        [Category("Spellchecker")]
        public String Thesaurus { get; set; }

        [Description("Custom words added to dictionary. Requires restart to apply.")]
        [Category("Spellchecker")]
        public List<String> CustomDictionaryEntries { get; set; }

        [Description("Words to hilite.")]
        //[Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        //    "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        public List<HiliteWord> SpecialWords { get; set; }

        [Description("Should a backup be made each time you save?")]
        public bool BackupOnSave { get; set; }
        
        private System.Drawing.Font _editorFont = null;

        [Description("Font to use for GUI elements. Requires restart to apply.")]
        public System.Drawing.Font SystemFont { get; set; }

        public List<Style> Styles { get; set; }
        
        public System.Drawing.Font TextEditorFont 
        {
            get
            {
                return _editorFont;
            }
            set
            {
                _editorFont = value;
                if (OnSettingsChanged != null) OnSettingsChanged();
            }
        }

        public Settings()
        {
            Dictionary = "en_US";
            Thesaurus = "th_en_US_new.dat";
            CustomDictionaryEntries = new List<string>();
            SpecialWords = new List<HiliteWord>();
            RecentDocuments = new List<string>();
            TextEditorFont = new System.Drawing.Font("Arial", 12);
            
            BackupOnSave = true;

            SystemFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
        }

        public void Verify()
        {
            if (Styles.Count == 0)
                Styles = new List<Style>(new Style[]
                {
                    new Style {},
                    new Style { Italic = true },
                    new Style { ForeColor = System.Drawing.Color.Red, Italic = true }
                });
        }
    }
}