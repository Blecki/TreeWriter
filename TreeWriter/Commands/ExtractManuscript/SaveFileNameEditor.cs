using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF.Commands.Extract
{
    public class SaveFileNameEditor : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null)
                return base.EditValue(context, provider, value);

            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                if (value != null)
                    saveFileDialog.FileName = value.ToString();

                saveFileDialog.Title = context.PropertyDescriptor.DisplayName;
                saveFileDialog.Filter = "TEXT files (*.txt)|*.txt";
                saveFileDialog.CheckFileExists = false;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    value = saveFileDialog.FileName;
            }

            return value;
        }
    }
}
