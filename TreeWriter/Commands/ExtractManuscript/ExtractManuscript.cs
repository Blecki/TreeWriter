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
        private ExtractionSettings Settings;
        public bool Succeeded { get; private set; }

        public ExtractManuscript(ManuscriptData Document, ExtractionSettings Settings)
        {
            this.Document = Document;
            this.Settings = Settings;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            if (System.IO.File.Exists(Settings.DestinationFile))
                if (System.Windows.Forms.MessageBox.Show(
                    String.Format("{0} already exists. Overwrite?", Settings.DestinationFile), 
                    "Warning", System.Windows.Forms.MessageBoxButtons.YesNo) 
                    != System.Windows.Forms.DialogResult.Yes)
                    return;

            ManuscriptFormatter formatter = null;
            switch (Settings.Format)
            {
                case ExtractionSettings.Formats.PlainText:
                    formatter = new PlainTextFormatter();
                    break;
                case ExtractionSettings.Formats.Latex:
                    formatter = new LatexFormatter();
                    break;
            }

            var chapters = !String.IsNullOrEmpty(Settings.ChapterTag);
            var chapter = 0;
            var scenesInChapter = 0;

            foreach (var scene in Document.Scenes)
            {
                if (chapters && scene.Tags.Contains(Settings.ChapterTag))
                {
                    chapter += 1;
                    formatter.BeginChapter(String.Format("Chapter {0}", chapter));
                    scenesInChapter = 0;
                }

                if (scenesInChapter > 0)
                    formatter.AddSceneBreak(Settings.SceneSeperator);

                formatter.AddScene(scene.Summary);

                scenesInChapter += 1;
            }

            System.IO.File.WriteAllText(Settings.DestinationFile, formatter.ToString());
            Succeeded = true;
        }
    }
}
