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
        /// <summary>
        /// Windows user application data directory
        /// </summary>
        string appdata;
        /// <summary>
        /// Subdirectory of user application data for ID Card Maker
        /// </summary>
        string appdir;


        /// <summary>
        /// Constructor for <code>MainWindow</code>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appdir = System.IO.Path.Combine(appdata, @"Dan Leonard\ID-Card-Maker");

            foreach (CardPreview.Design design in cardPreviewer.Designs)
            {
                // add designs to radio buttons
                RadioButtonDesign picker = new RadioButtonDesign();
                picker.Content = design.canonical_title;
                picker.Design = design;
                picker.Margin = new Thickness(10);
                RoutedEventArgs args = new RoutedEventArgs();
                picker.Checked += new RoutedEventHandler(cardPreviewer.SetDesign);

                CardDesignChoosers.Children.Add(picker);



                // add settings options to menu bar
                MenuItem menuItem = new MenuItem
                {
                    Header = "_" + design.canonical_title,
                    IsCheckable = true,
                };
                MenuItemExtensions.SetGroupName(menuItem, "Settings_Default");

                MenuItem_Settings_Default.Items.Add(menuItem);
            }

            // load from user setting
            int setting = Properties.Settings.Default.Design;
            (CardDesignChoosers.Children[setting] as RadioButtonDesign).IsChecked = true;
            (MenuItem_Settings_Default.Items[setting] as MenuItem).IsChecked = true;
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
            if (cardPreviewer.Footer.Children.Count != 0 )
            {
                try
                {
                    ((cardPreviewer.Footer.Children[0] as Panel).Children[0] as Label).Content =
                    "Admitted " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString();

                    Print(cardPreviewer, 1);

                    ((cardPreviewer.Footer.Children[0] as Panel).Children[0] as Label).Content = "Print Date";
                }
                catch (NullReferenceException ex)
                {
#if DEBUG
                    MessageBox.Show(ex.Message, "Exception caught", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                }
                catch (Exception ex)
                {
#if DEBUG
                    MessageBox.Show(ex.Message, "Exception caught", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    Print(cardPreviewer, 1);
                }
            }
            else
            {
                Print(cardPreviewer, 1);
            }
        }

        /// <summary>
        /// Method for safe printing all on one page
        /// </summary>
        /// <param name="v">Visual to be printed</param>
        /// <param name="page">Page to be printed</param>
        private void Print(Visual v, int page)
        {

            System.Windows.FrameworkElement e = v as System.Windows.FrameworkElement;
            if (e == null)
                return;

            PrintDialog pd = new PrintDialog();

            // mandate specific page
            pd.UserPageRangeEnabled = false;
            pd.PageRange = new PageRange(page);

            // set to id maker
            try
            {
                pd.PrintQueue = new System.Printing.PrintQueue(new System.Printing.PrintServer(), Properties.Resources.PrinterName);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show("Exception caught during printer choosing.\r\n\r\n" +
                                ex.Message +
                                "\r\nCurrent printer in Properties.Resources: " +
                                Properties.Resources.PrinterName + ".",
                                "Exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
#else
                MessageBoxResult exRes = MessageBox.Show("Ensure your " +
                                                            Properties.Resources.PrinterName +
                                                            " is plugged in and ready." +
                                                            "\r\n\r\nPrint to another printer?",
                                                            "Error printing.",
                                                            MessageBoxButton.OKCancel,
                                                            MessageBoxImage.Error,
                                                            MessageBoxResult.Cancel);
                switch (exRes)
                {
                    case MessageBoxResult.OK:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
#endif
            }

            if (pd.ShowDialog().Value)
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

        /// <summary>
        /// Save biodata to AppData
        /// </summary>
        private void ArchiveData()
        {
            Bio person = App.Current.Resources["person"] as Bio;

            if (person.Photo.Height != 1080.1507568359375) // this is very bad code
            {
                string filename = string.Concat(person.Name_First, "_", person.Name_Last, ".png");

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

        /// <summary>
        /// Closing process for <code>MainWindow</code>
        /// </summary>
        /// <remarks>
        /// Also manually terminates all running threads.
        /// </remarks>
        /// <seealso cref="App.wackoshutdown"/>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save user settings
            Properties.Settings.Default.Save();

            // pass close message to phototaker
            uc_PhotoTaker.Window_Closing(sender: sender, e: e);

            // kill all active threads
            Environment.Exit(0);
        }

        /// <summary>
        /// Click handler for About button in menu bar
        /// </summary>
        /// <remarks>
        /// Opens About pane
        /// </remarks>
        private void MenuItem_Help_About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        /// <summary>
        /// Click handler for Report Issues in menu bar
        /// </summary>
        /// <remarks>
        /// Opens GitHub issues page for project
        /// </remarks>
        private void MenuItem_Help_Report_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com./CombustibleLemon/ID-Card-Maker/issues");
        }

        private void MenuItem_File_AppData_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(appdir);
        }
    }



    public class Bio
    {
        public string Name_First {
            get;
            set;
        }
        public string Name_Last  {
            get;
            set;
        }
        public int Height_Feet { get; set; }
        public int Height_Inches { get; set; }
        public int Weight { get; set; }
        public DateTime Birthday { get; set; }
        public string Job_Title  { get; set; }
        public BitmapSource Photo { get; set; }
    }
}
