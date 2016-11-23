using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
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

    public class ManuscriptData
    {
        public String Version = "V1.0";
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

            if (r.Version != "V1.0")
                throw new Exception("Manuscript version not recognized.");

            if (r.Scenes == null) r.Scenes = new List<SceneData>();
            foreach (var scene in r.Scenes) scene.Validate();
            return r;
        }
    }

}
