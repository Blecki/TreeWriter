using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class ManuscriptDocument : EditableDocument
    {
        public ManuscriptData Data;
        public List<EditableDocument> OpenScenes = new List<EditableDocument>();

        public override void Load(Model Model, Main View, string Path)
        {
            this.Path = Path;

            var json = System.IO.File.ReadAllText(Path);
            if (String.IsNullOrEmpty(json))
                Data = ManuscriptData.CreateBlank();
            else
            {
                var versionEnd = json.IndexOfAny(new char[] { '\r', '\n' });
                var version = json.Substring(0, versionEnd);

                if (version == "{")
                {
                    var legacy = ManuscriptDataLegacyA.CreateFromJson(json);
                    if (System.Windows.Forms.MessageBox.Show("This manuscript is in a legacy format. It must be converted to open.", "Warning!", System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        Data = ManuscriptData.CreateFromLegacy(legacy);
                    }
                    else
                        throw new InvalidOperationException("Manuscript upgrade failed.");
                }
                else if (version == "V1.0")
                {
                    Data = ManuscriptData.CreateFromJson(json.Substring(versionEnd + 1));
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Unknown version!");
                    throw new InvalidOperationException("Unknown manuscript version.");
                }

            }
        }

        public override int CountWords(Model Model, Main View)
        {
            return Data.Scenes.Select(s => WordParser.CountWords(s.Prose)).Sum();
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new ManuscriptDocumentEditor(this);
            OpenEditors.Add(r);
            return r;
        }

        public override void Save(bool Backup)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented);
            var contents = ManuscriptData.CurrentVersionString + "\n" + json;
            System.IO.File.WriteAllText(Path, contents);
            NeedChangesSaved = false;
            UpdateViewTitles();

            foreach (var openScene in OpenScenes)
            {
                openScene.ClearChangesFlag();
                openScene.UpdateViewTitles();
            }

            if (Backup)
                System.IO.File.WriteAllText(GetBackupFilename(), contents);
        }

        protected override string ImplementGetEditorTitle()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Path);
        }
    }
}
