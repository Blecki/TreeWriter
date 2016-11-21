using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class SendToScrap :ICommand
    {
        private String Text;
        private String DocumentPath;
        public bool Succeeded { get; private set; }

        public SendToScrap(String Text, String DocumentPath)
        {
            this.Text = Text;
            this.DocumentPath = DocumentPath;
            Succeeded = true;
        }

        public void Execute(Model Model, Main View)
        {
            var scrap = String.Format("\nScrapped {0}\n{1}\n\n", DateTime.Now, Text);
            var scrapFileName = System.IO.Path.GetDirectoryName(DocumentPath) + "\\scrap.txt";

            var openScrapDocument = Model.FindOpenDocument(scrapFileName) as TextDocument;
            if (openScrapDocument != null)
            {
                openScrapDocument.ApplyChanges(openScrapDocument.Contents + scrap);
                openScrapDocument.UpdateViews();
            }
            else
                System.IO.File.AppendAllText(scrapFileName, scrap);

        }
    }
}
