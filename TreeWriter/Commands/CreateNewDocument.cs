using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class CreateNewDocument : ICommand
    {
        private String DirectoryPath;
        public String NewFileName;
        public bool Succeeded { get; private set; }
        
        public CreateNewDocument(String DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
            Succeeded = false;
        }

        public void Execute(Model Model, Main View)
        {
            var counter = 0;
            
            while (true)
            {
                try
                {
                    if (counter == 0) NewFileName = "untitled.txt";
                    else NewFileName = String.Format("untitled {0}.txt", counter);
            
                    var fullPath = DirectoryPath + "\\" + NewFileName;
                    if (System.IO.File.Exists(fullPath)) continue;

                    var file = System.IO.File.CreateText(fullPath);
                    file.Close();

                    break;
                }
                catch (System.IO.IOException e)
                {

                }
                finally
                {
                    counter += 1;
                }
            }

            Succeeded = true;
            // Guess the directory listing will update itself?
        }
    }
}
