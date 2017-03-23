using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF
{
    public class ManuscriptData
    {
        public const String CurrentVersionString = "V1.1";

        public Commands.Extract.ExtractionSettings ExtractionSettings = new Commands.Extract.ExtractionSettings();
        public List<SceneData> Scenes;
        public int WordCountGoal { get; set; }

        public static ManuscriptData CreateBlank()
        {
            return new ManuscriptData
            {
                Scenes = new List<SceneData>(),
                WordCountGoal = 100000
            };
        }

        public static ManuscriptData CreateFromJson(String Json)
        {
            var r = Newtonsoft.Json.JsonConvert.DeserializeObject<ManuscriptData>(Json);
            if (r == null) 
                throw new Exception("Failed to deserialize manuscript.");

            if (r.Scenes == null) r.Scenes = new List<SceneData>();
            foreach (var scene in r.Scenes) scene.Validate();
            return r;
        }

        public static ManuscriptData CreateFromLegacy(ManuscriptDataLegacyB Legacy)
        {
            return new ManuscriptData
            {
                Scenes = Legacy.Scenes.Select(scene => new SceneData { 
                    Name = scene.Name,
                    Color = scene.Color, 
                    Prose = scene.Prose,
                    SkipOnExtract = scene.SkipOnExtract,
                    StopExtractionHere = scene.StopExtractionHere,
                    Tags = scene.Tags,
                    DraftStatus = 0,
                    StartsNewChapter = scene.StartsNewChapter,
                    ChapterName = scene.ChapterName
                }).ToList(),
                ExtractionSettings = Commands.Extract.ExtractionSettings.CreateFromLegacy(Legacy.ExtractionSettings)
            };
        }
    }

}
