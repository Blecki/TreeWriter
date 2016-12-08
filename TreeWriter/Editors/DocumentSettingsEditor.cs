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
    public partial class DocumentSettingsEditor : DocumentEditor
    {
        Object SettingsObject;
        public Action OnObjectPropertyChange;

        public DocumentSettingsEditor(EditableDocument Document, Object SettingsObject) : base(Document)
        {
            this.Document = Document;
            this.SettingsObject = SettingsObject;

            this.InitializeComponent();

            propertyGrid.SelectedObject = SettingsObject;

            Text = Document.GetTitle();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Document.MadeChanges();
            if (OnObjectPropertyChange != null) OnObjectPropertyChange();
        }

        private void settings_KeyDown(object sender, KeyEventArgs e)
        {            
            base.DocumentEditor_KeyDown(sender, e);
        }

        
    }
}
