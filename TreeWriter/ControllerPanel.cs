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
    public partial class ControllerPanel : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public Action<ICommand> ControllerCommand;

        public ControllerPanel()
        {
            InitializeComponent();
        }

        public virtual void ReloadDocument()
        { }
    }
}
