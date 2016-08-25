using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CreateFolder :ICommand
    {
        private String DirectoryPath;
        public String NewFileName;

        public CreateFolder(String DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
        }

        public void Execute(Model Model, Main View)
        {
            // Create the file.
            var counter = 0;
            
            while (true)
            {
                try
                {
                    if (counter == 0) NewFileName = "new folder";
                    else NewFileName = String.Format("new folder {0}", counter);
                    counter += 1;

                    var fullPath = DirectoryPath + "\\" + NewFileName;
                    if (System.IO.Directory.Exists(fullPath)) continue;

                    var file = System.IO.Directory.CreateDirectory(fullPath);

                    break;
                }
                catch (System.IO.IOException e)
                {

                }
            }

            // Guess the directory listing will update itself?
        }
    }
}
