using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF
{
    public class SceneData
    {
        public String Name { get; set; }
        public String Tags { get; set; }
        public String Prose = "";
        public int DraftStatus { get; set; }
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
            DraftStatus = 0;
        }

        internal void Validate()
        {
            if (Name == null) Name = "";
            if (Tags == null) Tags = "";
            if (Prose == null) Prose = "";
        }
    }
}
