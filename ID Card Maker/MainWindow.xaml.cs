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
        Window windowDesign;
        CardPreview cardPreviewer;

        Bio person = new Bio
        {
            Name_First = "John",
            Name_Last = "Doe",
            Job_Title = "Important stuff",
            Photo = new BitmapImage(new Uri(@"assets/img/unkown person.png", UriKind.Relative))
        };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = person;
            cardPreviewer = new CardPreview()
            {
                DataContext = person
            };
        }

        /// <summary>
        /// Open a second window containing the CardPreview object
        /// </summary>
        private void ShowPreviewer()
        {
            windowDesign = new Window()
            {
                Title = "Print Preview",
                SizeToContent = SizeToContent.WidthAndHeight,
                Content = cardPreviewer,
                ResizeMode = ResizeMode.NoResize,
            };
            windowDesign.Show();
        }

        /// <summary>
        /// Open system print dialog and send it the card preview
        /// </summary>
        private void InvokePrint(object sender, RoutedEventArgs e)
        {
            // Create a print dialog object
            PrintDialog dialog = new PrintDialog();
            dialog.UserPageRangeEnabled = true;

            if (dialog.ShowDialog() == true)
            {
                // Print the card design
                dialog.PrintVisual(cardPreviewer, "ID Card");
            }
        }

        /// <summary>
        /// Either show or hide the print preview pane
        /// </summary>
        private void PreviewRequest(object sender, RoutedEventArgs e)
        {
            if ( windowDesign == null )
            {
                ShowPreviewer();
                PrintPreview.Content = "Hide Preview";
            }
            else
            {
                windowDesign.Close();
                windowDesign = null;
                PrintPreview.Content = "Show Preview";
            }
        }

        private void Btn_Photo_Click(object sender, RoutedEventArgs e)
        {
            PhotoTaker photoWindow = new PhotoTaker()
            {
                DataContext = this
            };
            photoWindow.Show();
        }

        private void Input_Text_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }

    public class Bio
    {
        public string Name_First { get; set; }
        public string Name_Last { get; set; }
        public string Job_Title { get; set; }
        public BitmapSource Photo { get; set; }
    }
}
