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

            propertyGrid.SelectedObject = Settings.GlobalSettings;

            Settings.GlobalSettings.OnSettingsChanged = null;
            Settings.GlobalSettings.OnSettingsChanged += OnFontChanged;
        }       

        private void OnFontChanged()
        {
            foreach (var openDocument in Model.EnumerateOpenDocuments())
                foreach (var openView in openDocument.OpenEditors)
                    openView.ReloadSettings();
        }

        private void SettingsEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Open = false;
        }
    }
}
