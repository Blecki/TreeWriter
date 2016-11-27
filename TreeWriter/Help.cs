using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ScintillaNET;

namespace TreeWriterWF
{
    public partial class Help : DockablePanel
    {
        public bool Open = true;

        public Help()
        {
            this.InitializeComponent();
        }       

        private void SettingsEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Open = false;
        }

        public void Navigate(String To)
        {
            while (webBrowser.IsBusy) { }

            webBrowser.Navigate(
                String.Format("file:///{0}/help/{1}.html", System.IO.Directory.GetCurrentDirectory(), To));
            Text = "#" + To;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Text = "#" + webBrowser.Document.Title;
        }
    }
}
