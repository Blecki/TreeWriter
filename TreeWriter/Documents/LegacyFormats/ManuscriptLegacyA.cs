using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class ManuscriptDataLegacyA
    {
        public class SceneData
        {
            public String Name = "";
            public String Tags = "";
            public String Summary = "";
            public int Color = -1;

            internal void Validate()
            {
                if (Name == null) Name = "";
                if (Tags == null) Tags = "";
                if (Summary == null) Summary = "";
            }
        }

        public class _ExtractionSettings
        {
            public String DestinationFile { get; set; }
            public String SceneSeperator { get; set; }
            public String ChapterTag { get; set; }

            public enum Formats
            {
                PlainText,
                Latex
            }

            public Formats Format { get; set; }
        }

        public String Version = "V1.0";
        public _ExtractionSettings ExtractionSettings = new _ExtractionSettings();
        public List<SceneData> Scenes;

        public static ManuscriptDataLegacyA CreateFromJson(String Json)
        {
            var r = Newtonsoft.Json.JsonConvert.DeserializeObject<ManuscriptDataLegacyA>(Json);
            if (r == null) 
                throw new Exception("Failed to deserialize manuscript.");

            if (r.Version != "V1.0")
                throw new Exception("Manuscript version not recognized.");

            if (r.Scenes == null) r.Scenes = new List<SceneData>();
            foreach (var scene in r.Scenes) scene.Validate();
            return r;
        }
    }

}
