using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
//AForge.Video dll
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;
using System.Drawing;

namespace ID_Card_Maker
{
    /// <summary>
    /// Interaction logic for PhotoTaker.xaml
    /// </summary>
    public partial class PhotoTaker : UserControl
    {
        private bool ready = false;
        private bool isDragging = false;
        private System.Drawing.Point anchorPoint;
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;

        private MouseButtonEventHandler mouseEvent_ButtonDown;
        private MouseEventHandler mouseEvent_Move;
        private MouseButtonEventHandler mouseEvent_ButtonUp;
        private NewFrameEventHandler frameHandler;

        /// <summary>
        /// Construct new instance of <code>PhotoTaker</code> window object
        /// </summary>
        public PhotoTaker()
        {
            InitializeComponent();
            Image_Previewer.Source = null;

            frameHandler = new NewFrameEventHandler(FinalFrame_NewFrame);
            mouseEvent_ButtonDown = new MouseButtonEventHandler(Image_Previewer_MouseLeftButtonDown);
            mouseEvent_Move = new MouseEventHandler(Image_Previewer_MouseMove);
            mouseEvent_ButtonUp = new MouseButtonEventHandler(Image_Previewer_MouseLeftButtonUp);
        }

        /// <summary>
        /// Event handler for <code>PhotoTaker</code> window object loading
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);//constructor
            foreach (FilterInfo Device in CaptureDevice)
            {
                ComboBox_DeviceChooser.Items.Add(Device.Name);
            }

            ComboBox_DeviceChooser.SelectedIndex = 0; // default
            //FinalFrame = new VideoCaptureDevice();
        }

        /// <summary>
        /// Event handler for click of button adjoining <code>ComboBox_DeviceChooser</code>
        /// </summary>
        private void Button_DeviceChooser_Click(object sender, RoutedEventArgs e)
        {
            if(ComboBox_DeviceChooser.SelectedIndex < 0)
            {
                return;
            }
            FinalFrame = new VideoCaptureDevice(CaptureDevice[ComboBox_DeviceChooser.SelectedIndex].MonikerString); // specified web cam and its filter moniker string
            FinalFrame.NewFrame += frameHandler; // click button event is fired, 
            FinalFrame.VideoSourceError += new VideoSourceErrorEventHandler(videoSource_Error);
            anchorPoint = new System.Drawing.Point();
            selectionRectangle.Visibility = Visibility.Hidden;

            ready = true;
            Button_Crop.IsEnabled = false;
            Button_Capture.IsEnabled = true;

            Image_Previewer.Cursor = null;
            FinalFrame.Start();

            // Remove crop handlers
            Image_Previewer.MouseLeftButtonDown -= mouseEvent_ButtonDown;
            Image_Previewer.MouseMove -= mouseEvent_Move;
            Image_Previewer.MouseLeftButtonUp -= mouseEvent_ButtonUp;
        }

        /// <summary>
        /// Event handler for <code>VideoCaptureDevice</code> <code>FinalFrame</code> event
        /// </summary>
        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs) // must be void so that it can be accessed everywhere.
                                                                             // New Frame Event Args is an constructor of a class
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    BitmapSource bs = null;

                    Bitmap bm = AForge.Imaging.Image.Clone(eventArgs.Frame);

                    // memory leak stuff, see class SafeHBitmapHandle

                    IntPtr hBitmap = bm.GetHbitmap();
                    var handlebitmap = new SafeHBitmapHandle(hBitmap, true);
                    
                    try
                    {
                        using (handlebitmap)
                        {
                            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                handlebitmap.DangerousGetHandle(),
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromWidthAndHeight(bm.Width, bm.Height));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, ex.Message, ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (ready)
                        Image_Previewer.Source = bs;
                    bm.Dispose();
                });
            } catch {} //this is bad code too
            // clone the bitmap
        }

        /// <summary>
        /// Convert <code>Bitmap</code> object to <code>ImageSource</code>
        /// </summary>
        /// <param name="bitmap">A <code>Bitmap</code> object to be converted</param>
        /// <returns><code>ImageSource</code> object corresponding to the provided <code>Bitmap</code></returns>
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// Event handler for closing of <code>PhotoTaker</code> window
        /// </summary>
        public void Window_Closing(object sender, CancelEventArgs e)
        {
            Button_DeviceChooser_Click(sender, null);
            if (FinalFrame.IsRunning == true)
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        FinalFrame.Stop();
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.ToString());
                }
            }
        }

        /// <summary>
        /// Event handler for clicking 'capture' button
        /// </summary>
        private void Button_Capture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Capture();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "Exception caught.\r\n" + ex.Message,
                    "Null Reference Exception",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK
                    );
            }
        }

        /// <summary>
        /// Add event handlers to HolyGrid and prepare for cropping
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        /// Thrown when attempting to capture image while <code>Image_Previewer.Source</code> is empty
        /// </exception>
        private void Capture()
        {
            if (Image_Previewer.Source != null)
            {
                // Disable update from camera
                ready = false;
                Image_Previewer.Cursor = Cursors.Cross;

                // Add event handlers for cropping
                HolyGrid.MouseLeftButtonDown += mouseEvent_ButtonDown;
                HolyGrid.MouseMove += mouseEvent_Move;
                HolyGrid.MouseLeftButtonUp += mouseEvent_ButtonUp;
            }
            else
            {
                throw new NullReferenceException("Attempted to capture image from empty feed.");
            }
        }

        #region Crop event handlers
        private void Image_Previewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                double x = e.GetPosition(BackPanel).X;
                double y = e.GetPosition(BackPanel).Y;
                selectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, anchorPoint.X));
                selectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, anchorPoint.Y));
                selectionRectangle.Width = Math.Abs(x - anchorPoint.X);
                selectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

                if (selectionRectangle.Visibility != Visibility.Visible)
                    selectionRectangle.Visibility = Visibility.Visible;
            }
        }

        private void Image_Previewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDragging == false)
            {
                anchorPoint.X = (int)e.GetPosition(BackPanel).X;
                anchorPoint.Y = (int)e.GetPosition(BackPanel).Y;
                isDragging = true;
            }
        }

        private void Image_Previewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                if (selectionRectangle.Width > 0)
                {
                    Button_Crop.Visibility = System.Windows.Visibility.Visible;
                    Button_Crop.IsEnabled = true;
                }
                if (selectionRectangle.Visibility != Visibility.Visible)
                    selectionRectangle.Visibility  = Visibility.Visible;
            }
        }
        #endregion

        private void Button_Crop_Click(object sender, RoutedEventArgs e)
        {
            if (Image_Previewer.Source != null)
            {
                double iX = Image_Previewer.Width;
                double iY = Image_Previewer.Height;
                double pX = Image_Previewer.Source.Width;
                double pY = Image_Previewer.Source.Height;
                double letterbox = (iY - pY) / 2;
                double pillarbox = (iX - pX) / 2;

                Rect rect1 = new Rect(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle), selectionRectangle.Width, selectionRectangle.Height);
                System.Windows.Int32Rect rcFrom = new System.Windows.Int32Rect();
                rcFrom.X = (int)((rect1.X) /* - pillarbox */);
                rcFrom.Y = (int)((rect1.Y) /* - letterbox */);
                rcFrom.Width = (int)((rect1.Width) /* * (Image_Previewer.Source.Width) / (Image_Previewer.Width) */);
                rcFrom.Height = rcFrom.Width;  
                BitmapSource bs = new CroppedBitmap(Image_Previewer.Source as BitmapSource, rcFrom);

                SaveImage(image: bs);

                Button_DeviceChooser_Click(sender, e);
            }
        }

        /// <summary>
        /// Bubble up cropped image into the main program
        /// </summary>
        /// <param name="image">The cropped ID photo to use</param>
        private void SaveImage(BitmapSource image)
        {
            //((MainWindow)Application.Current.MainWindow).Input_Photo.Source = image;
            Photo.Source = image;
            Image_Previewer.Source = image;
        }

        void videoSource_Error(object sender, AForge.Video.VideoSourceErrorEventArgs eventArgs)
        {
#if DEBUG
            MessageBox.Show(
                eventArgs.Description,
                eventArgs.GetType().ToString(),
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
#endif
        }
    }

    /// <summary>
    /// Safe handlers for HBitmap
    /// </summary>
    /// <remarks>
    /// Adapted from various StackOverflow answers at
    /// https://stackoverflow.com/questions/1546091/wpf-createbitmapsourcefromhbitmap-memory-leak
    /// </remarks>
    public class SafeHBitmapHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr hObject);

        [System.Security.SecurityCritical]
        public SafeHBitmapHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return DeleteObject(handle) > 0;
        }
    }
}
