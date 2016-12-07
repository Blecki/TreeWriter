using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF.Commands.Extract
{
    public class ExtractionSettings
    {
        [Category("File")]
        [Description("Destination file for extracted manuscript.")]
        [Editor(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public String DestinationFile { get; set; }

        [Description("This text is placed between scenes.")]
        public String SceneSeperator { get; set; }

        public enum Formats
        {
            PlainText,
            Latex
        }

        public Formats Format { get; set; }

        public void SetDefaultPath(ManuscriptDocument Document)
        {
            DestinationFile = System.IO.Path.GetDirectoryName(Document.Path) + "\\" +
                System.IO.Path.GetFileNameWithoutExtension(Document.Path) +
                "_extracted.txt";
        }

        public ExtractionSettings()
        {
            SceneSeperator = "* * *";
            Format = Formats.PlainText;
        }
    }
}
