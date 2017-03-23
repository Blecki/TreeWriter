using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF
{
    public class ManuscriptDataLegacyB
    {
        public class SceneData
        {
            public String Name { get; set; }
            public String Tags { get; set; }
            public String Prose = "";
            public String Status;
            public System.Drawing.Color Color { get; set; }

            [Description("Does this scene begin a new chapter? If true, set the chapter name attribute.")]
            [Category("Chapter")]
            public bool StartsNewChapter { get; set; }

            [Category("Chapter")]
            public String ChapterName { get; set; }

            public bool SkipOnExtract { get; set; }
            public bool StopExtractionHere { get; set; }

            public SceneData()
            {
                Name = "";
                Tags = "";
                Color = System.Drawing.Color.White;
                Prose = "";
                StartsNewChapter = false;
                ChapterName = "";
                SkipOnExtract = false;
                StopExtractionHere = false;
                Status = "SUMMARY";
            }

            internal void Validate()
            {
                if (Name == null) Name = "";
                if (Tags == null) Tags = "";
                if (Prose == null) Prose = "";
                if (Status == null) Status = "SUMMARY";
            }
        }

        public class CExtractionSettings
        {
            public String DestinationFile { get; set; }
            public String SceneSeperator { get; set; }

            public enum Formats
            {
                PlainText,
                Latex
            }

            public Formats Format { get; set; }

            public CExtractionSettings()
            {
                SceneSeperator = "* * *";
                Format = Formats.PlainText;
            }
        }

        public CExtractionSettings ExtractionSettings = new CExtractionSettings();
        public List<SceneData> Scenes;
        public int WordCountGoal { get; set; }

        public static ManuscriptDataLegacyB CreateFromJson(String Json)
        {
            var r = Newtonsoft.Json.JsonConvert.DeserializeObject<ManuscriptDataLegacyB>(Json);
            if (r == null) 
                throw new Exception("Failed to deserialize manuscript.");

            if (r.Scenes == null) r.Scenes = new List<SceneData>();
            foreach (var scene in r.Scenes) scene.Validate();
            return r;
        }

        public static ManuscriptDataLegacyB CreateFromLegacy(ManuscriptDataLegacyA Legacy)
        {
            return new ManuscriptDataLegacyB
            {
                Scenes = Legacy.Scenes.Select(scene => new SceneData { Name = scene.Name, Color = System.Drawing.Color.FromArgb(scene.Color), Prose = scene.Summary, Tags = scene.Tags }).ToList(),
                ExtractionSettings = new CExtractionSettings()
            };
        }
    }

}
