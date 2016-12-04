using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF.Commands
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

    public class ExtractionSettings
    {
        [Category("File")]
        [Description("Destination file for extracted manuscript.")]
        [Editor(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public String DestinationFile { get; set; }

        public int LinesBetweenScenes { get; set; }
        public String SceneSeperator { get; set; }

        public enum Formats
        {
            PlainText,
            Latex
        }

        public Formats Format { get; set; }

        public ExtractionSettings(ManuscriptDocument Document)
        {
            DestinationFile = System.IO.Path.GetDirectoryName(Document.Path) +
                System.IO.Path.GetFileNameWithoutExtension(Document.Path) +
                "_extracted.txt";

            LinesBetweenScenes = 2;
            SceneSeperator = "* * *";
            Format = Formats.Latex;
        }
    }

    public class OpenManuscriptExtractor : ICommand
    {
        private ManuscriptDocument Document;
        public bool Succeeded { get; private set; }

        public OpenManuscriptExtractor(ManuscriptDocument Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            ManuscriptExtractor existing = Document.OpenEditors.FirstOrDefault(v => v is ManuscriptExtractor) as ManuscriptExtractor;
            if (existing == null)
            {
                existing = new ManuscriptExtractor(Document);
                View.OpenControllerPanel(existing, WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
                Document.OpenEditors.Add(existing);
            }
            existing.Focus();
            Succeeded = true;
        }
    }

    public class ExtractManuscript :ICommand
    {
        private ManuscriptData Document;
        private ExtractionSettings Settings;
        public bool Succeeded { get; private set; }

        public ExtractManuscript(ManuscriptData Document, ExtractionSettings Settings)
        {
            this.Document = Document;
            this.Settings = Settings;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (System.IO.File.Exists(Settings.DestinationFile))
                if (System.Windows.Forms.MessageBox.Show(String.Format("{0} already exists. Overwrite?", Settings.DestinationFile), "Warning", System.Windows.Forms.MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;
            var contents = new StringBuilder();
            foreach (var scene in Document.Scenes)
                contents.Append(scene.Summary + new String('\n', Settings.LinesBetweenScenes));
            System.IO.File.WriteAllText(Settings.DestinationFile, contents.ToString());
            Succeeded = true;
        }
    }
}
