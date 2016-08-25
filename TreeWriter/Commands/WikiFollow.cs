using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF.Commands
{
    public class WikiFollow :ICommand
    {
        private Document Origin;
        public String ArticleName;
        
        public WikiFollow(Document Origin, String ArticleName)
        {
            this.Origin = Origin;
            this.ArticleName = ArticleName;
        }

        public void Execute(Model Model, Main View)
        {
            var results = Model.WikiSearch(System.IO.Path.GetDirectoryName(Origin.Owner.Path), ArticleName);
            if (String.IsNullOrEmpty(results))
            {
                if (System.Windows.Forms.MessageBox.Show("Could not find article. Create as a sibling?", "!", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    var fileName = System.IO.Path.GetDirectoryName(Origin.FileName) + "\\" + ArticleName + ".txt";
                    System.Diagnostics.Debug.Assert(!System.IO.File.Exists(fileName));
                    System.IO.File.CreateText(fileName).Close();
                    
                    // Update project tree view.
                    if (Origin.Owner.OpenView != null) Origin.Owner.OpenView.UpdateNode(null);

                    (new OpenFile(fileName, Origin.Owner)).Execute(Model, View);
                }
            }
            else
            {
                var openCommand = new OpenFile(results, Origin.Owner);
                openCommand.Execute(Model, View);
            }
        }
    }
}
