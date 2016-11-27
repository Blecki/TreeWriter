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
    public partial class SettingsEditor : DockablePanel
    {
        Model Model;
        public bool Open = true;

        public SettingsEditor(Model Model)
        {
            this.Model = Model;

            this.InitializeComponent();

            propertyGrid.SelectedObject = Model.Settings;

            Model.Settings.OnEditorFontChanged = null;
            Model.Settings.OnEditorFontChanged += OnFontChanged;
        }       

        private void OnFontChanged()
        {
            foreach (var openDocument in Model.EnumerateOpenDocuments())
                foreach (var openView in openDocument.OpenEditors)
                    openView.ReloadSettings(Model.Settings);
        }

        private void SettingsEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Open = false;
        }
    }
}
