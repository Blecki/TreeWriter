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
        
        public override void Load(string Path)
        {
            this.Path = Path;

            var json = System.IO.File.ReadAllText(Path);

            if (String.IsNullOrEmpty(json))
                Data = ManuscriptData.CreateBlank();
            else
                Data = ManuscriptData.CreateFromJson(json);
        }

        public override int CountWords(Model Model, Main View)
        {
            return Data.Scenes.Select(s => WordParser.CountWords(s.Summary)).Sum();
        }

        public override DockablePanel OpenView(Model Model)
        {
            var r = new ManuscriptDocumentEditor(Model.Settings, this, Model.SpellChecker, Model.Thesaurus);
            OpenEditors.Add(r);
            return r;
        }

        public override void SaveDocument()
        {
            System.IO.File.WriteAllText(Path, Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented));
            NeedChangesSaved = false;
            UpdateViewTitles();
        }
    }
}
