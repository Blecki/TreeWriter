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

        public bool SendSceneToScrap(Model Model, SceneData Scene)
        {
            try
            {
                var scrap = String.Format("\nScrapped {0}\nName: {1}\nTags: {2}\nSummary: {3}\n\n",
                    DateTime.Now, Scene.Name, Scene.Tags, Scene.Summary);

                var scrapFileName = System.IO.Path.GetDirectoryName(Path) + "\\" +  System.IO.Path.GetFileNameWithoutExtension(Path) + "_ms_scrap.txt";

                var openScrapDocument = Model.FindOpenDocument(scrapFileName);
                if (openScrapDocument != null)
                {
                    openScrapDocument.ApplyChanges(openScrapDocument.GetContents() + scrap);
                    openScrapDocument.UpdateViews();
                }
                else
                    System.IO.File.AppendAllText(scrapFileName, scrap);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
