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
        public const String CurrentVersionString = "V1.0";

        public Commands.Extract.ExtractionSettings ExtractionSettings = new Commands.Extract.ExtractionSettings();
        public List<SceneData> Scenes;

        public static ManuscriptData CreateBlank()
        {
            return new ManuscriptData
            {
                Scenes = new List<SceneData>()
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

        public static ManuscriptData CreateFromLegacy(ManuscriptDataLegacyA Legacy)
        {
            return new ManuscriptData
            {
                Scenes = Legacy.Scenes.Select(scene => new SceneData { Name = scene.Name, Color = System.Drawing.Color.FromArgb(scene.Color), Prose = scene.Summary, Tags = scene.Tags }).ToList(),
                ExtractionSettings = new Commands.Extract.ExtractionSettings()
            };
        }
    }

}
