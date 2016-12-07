using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TreeWriterWF.Commands.Extract
{
    public class ExtractManuscript : ICommand
    {
        private ManuscriptData Document;
        public bool Succeeded { get; private set; }

        public ExtractManuscript(ManuscriptData Document)
        {
            this.Document = Document;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (System.IO.File.Exists(Document.ExtractionSettings.DestinationFile))
                if (System.Windows.Forms.MessageBox.Show(
                    String.Format("{0} already exists. Overwrite?", Document.ExtractionSettings.DestinationFile), 
                    "Warning", System.Windows.Forms.MessageBoxButtons.YesNo) 
                    != System.Windows.Forms.DialogResult.Yes)
                    return;

            ManuscriptFormatter formatter = null;
            switch (Document.ExtractionSettings.Format)
            {
                case ExtractionSettings.Formats.PlainText:
                    formatter = new PlainTextFormatter();
                    break;
                case ExtractionSettings.Formats.Latex:
                    formatter = new LatexFormatter();
                    break;
            }

            var chapters = !String.IsNullOrEmpty(Document.ExtractionSettings.ChapterTag);
            var chapter = 0;
            var scenesInChapter = 0;

            foreach (var scene in Document.Scenes)
            {
                if (chapters && scene.Tags.Contains(Document.ExtractionSettings.ChapterTag))
                {
                    chapter += 1;
                    formatter.BeginChapter(String.Format("Chapter {0}", chapter));
                    scenesInChapter = 0;
                }

                if (scenesInChapter > 0)
                    formatter.AddSceneBreak(Document.ExtractionSettings.SceneSeperator);

                formatter.AddScene(scene.Summary);

                scenesInChapter += 1;
            }

            System.IO.File.WriteAllText(Document.ExtractionSettings.DestinationFile, formatter.ToString());
            Succeeded = true;
        }
    }
}
