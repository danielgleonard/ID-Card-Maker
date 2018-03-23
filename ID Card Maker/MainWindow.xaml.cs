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

namespace ID_Card_Maker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NameFirst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            NameFirst.SelectAll();
        }

        private void NameLast_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NameLast.SelectAll();
        }

        private void JobDescription_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JobDescription.SelectAll();
        }
    }
}
