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

        private void NameFirst_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(NameFirst.Text == "First Name")
            {
                NameFirst.SelectAll();
            }
        }

        private void NameLast_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameLast.Text == "Last Name")
            {
                NameLast.SelectAll();
            }
        }

        private void JobDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (JobDescription.Text == "Last Name")
            {
                JobDescription.SelectAll();
            }
        }
    }
}
