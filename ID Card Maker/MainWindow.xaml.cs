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
                //Title = "Print Preview",
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
            /*
            // Create a print dialog object
            PrintDialog dialog = new PrintDialog();
            dialog.UserPageRangeEnabled = true;

            if (dialog.ShowDialog() == true)
            {
                // Print the card design
                //dialog.PrintVisual(cardPreviewer, "ID Card");
                PrintHelper.ShowPrintPreview(PrintHelper.GetFixedDocument(cardPreviewer, dialog));
            }
            */

            ArchiveData();
            Print(cardPreviewer);
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

        /// <summary>
        /// Method for safe printing all on one page
        /// </summary>
        /// <param name="v">Visual to be printed</param>
        private void Print(Visual v)
        {

            System.Windows.FrameworkElement e = v as System.Windows.FrameworkElement;
            if (e == null)
                return;

            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                //store original scale
                Transform originalScale = e.LayoutTransform;
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / e.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               e.ActualHeight);

                //Transform the Visual to scale
                e.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                e.Measure(sz);
                e.Arrange(new System.Windows.Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                pd.PrintVisual(v, "My Print");

                //apply the original transform.
                e.LayoutTransform = originalScale;
            }
        }

        private void ArchiveData()
        {
            if (person.Photo.Height != 1080.1507568359375) // this is very bad code
            {
                string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string filename = string.Concat(person.Name_First, "_", person.Name_Last, ".png");
                string appdir = System.IO.Path.Combine(appdata, @"Dan Leonard\ID-Card-Maker");

                SaveBitmapImageToFile(person.Photo, appdir, filename);
            }
        }

        /// <summary>
        /// Save Bitmap image to file
        /// </summary>
        /// <param name="image">The Bitmap to be saved</param>
        /// <param name="filePath">Path to save the file</param>
        /// <param name="fileName">Name of bitmap file</param>
        /// <remarks>
        /// Adapted from StackOverflow answer by Thomas Levesque
        /// </remarks>
        /// <see>https://stackoverflow.com/a/2900564</see>
        public static void SaveBitmapImageToFile(BitmapSource image, string filePath, string fileName)
        {
            System.IO.Directory.CreateDirectory(filePath);
            using (var fileStream = new System.IO.FileStream(filePath + '\\' + fileName, System.IO.FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
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
