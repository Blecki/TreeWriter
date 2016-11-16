using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public class SceneData
    {
        public String Name;
        public String Tags;
        public String Summary;
        public String Prose;
    }

    public class ManuscriptData
    {
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
            if (r == null) throw new Exception("Failed to deserialize manuscript.");
            return r;
        }
    }

}
