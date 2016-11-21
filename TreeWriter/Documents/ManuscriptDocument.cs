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

        public ManuscriptDocument(String Path, ManuscriptData Data)
        {
            this.Path = Path;
            this.Data = Data;
        }

        public override int CountWords()
        {
            return Data.Scenes.Select(s => WordParser.CountWords(s.Summary)).Sum();
        }

        public override OpenDocumentRecord GetOpenDocumentRecord()
        {
            return new OpenDocumentRecord
            {
                Path = Path,
                Type = "MANUSCRIPT"
            };
        }

        public override ControllerPanel OpenView(Model Model)
        {
            var r = new SceneListing(this);
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
