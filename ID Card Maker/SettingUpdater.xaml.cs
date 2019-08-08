using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ID_Card_Maker
{
    /// <summary>
    /// Interaction logic for SettingUpdater.xaml
    /// </summary>
    public partial class SettingUpdater : Window
    {
        private readonly string settingMachine;
        private readonly string settingHuman;

        /// <summary>
        /// Window to update a specific setting
        /// </summary>
        /// <param name="settingName">Name of setting to update</param>
        /// <param name="settingHumanReadable">Human-readable description of setting to update</param>
        public SettingUpdater(string settingName, string settingHumanReadable)
        {
            settingMachine = settingName;
            settingHuman = settingHumanReadable;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            titleBox.Text += String.Format(" “{0}”", arg0: settingHuman);
            settingBox.Text = Properties.Settings.Default[settingMachine] as string;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default[settingMachine] = settingBox.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }

}
