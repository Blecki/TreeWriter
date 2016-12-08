using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeWriterWF
{
    public partial class DistractionFreeEditor : Form
    {
        public DistractionFreeEditor()
        {
            InitializeComponent();
            Font = Settings.GlobalSettings.SystemFont;
        }
        
        public DistractionFreeEditor(System.Drawing.Font Font, ScintillaNET.Document LinkedDocument)
        {
            InitializeComponent();
            textEditor.Document = LinkedDocument;
            textEditor.IsDistractionFreeMode = true;

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            textEditor.LoadFont(Font);
        }
    }
}
