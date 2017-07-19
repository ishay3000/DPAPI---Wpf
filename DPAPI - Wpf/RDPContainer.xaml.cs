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
using System.Windows.Shapes;

namespace DPAPI___Wpf
{
    /// <summary>
    /// Interaction logic for RDPContainer.xaml
    /// </summary>
    public partial class RDPContainer : Window
    {
        private AxRDPCOMAPILib.AxRDPViewer rdp_viewer = new AxRDPCOMAPILib.AxRDPViewer();
        private string connString;

        public RDPContainer(string connectionString)
        {
            this.connString = connectionString;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.rdp_viewer.Connect(this.connString, "User1", "");
            }
            catch (Exception crap)
            {
                System.Windows.MessageBox.Show(crap.Message);
            }
        }
    }
}
