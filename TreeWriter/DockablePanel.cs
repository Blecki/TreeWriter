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
    public partial class DockablePanel : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action<ICommand> InvokeCommand;
        public EditableDocument.CloseStyle CloseStyle = EditableDocument.CloseStyle.Natural;

        public DockablePanel()
        {
            InitializeComponent();
        }

        public virtual void ReloadDocument()
        { }
    }
}
