using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;

namespace BrowserHost
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            var mainForm = new TreeWriterWF.Main();

            InitializeComponent();

            //Create a Windows Forms Host to host a form
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();

            stackPanel.Width = mainForm.Width;
            stackPanel.Height = mainForm.Height;
            windowsFormsHost.Width = mainForm.Width;
            windowsFormsHost.Height = mainForm.Height;

            mainForm.TopLevel = false;

            windowsFormsHost.Child = mainForm;

            stackPanel.Children.Add(windowsFormsHost);
        }
    }
}
