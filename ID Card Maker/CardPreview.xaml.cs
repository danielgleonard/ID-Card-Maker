using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for CardPreview.xaml
    /// </summary>
    public partial class CardPreview : UserControl
    {
        /// <summary>
        /// Design of card
        /// </summary>
        public List<Design> Designs = new List<Design>();
        public DateTime CurrentDateAndTime { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        Design Visitor_Photo = new Design
        {
            canonical_title = "Visitor Pass w/ Photo",
            logoref = "visitor_textDrawingImage",
            isPhoto = true,
            color_bg_primary = new SolidColorBrush { Color = Colors.White },
            color_bg_secondary = new SolidColorBrush { Color = Colors.Gray },
            color_text_primary = new SolidColorBrush { Color = Colors.Black },
            color_text_secondary = new SolidColorBrush { Color = Colors.Black },
        };

        Design Visitor_Gold_Rush = new Design
        {
            canonical_title = "Visitor for Gold Rush",
            logoref = "visitor_textDrawingImage",
            isPhoto = false,
            logo2ref = "gold_rush_text_vectorDrawingImage",
            color_bg_primary = new SolidColorBrush { Color = Colors.White },
            color_bg_secondary = new SolidColorBrush { Color = Colors.Black },
            color_text_primary = null,
            color_text_secondary = null,
        };

        Design Visitor_Heidner_Properties = new Design
        {
            canonical_title = "Visitor for Heidner Properties",
            logoref = "visitor_textDrawingImage",
            isPhoto = false,
            logo2ref = "heidner_vectorDrawingImage",
            color_bg_primary = new SolidColorBrush { Color = Colors.White },
            color_bg_secondary = new SolidColorBrush { Color = Colors.LightGray },
            color_text_primary = null,
            color_text_secondary = null,
        };

        Design Gold_Rush = new Design
        {
            canonical_title = "Gold Rush Gaming",
            logoref = "gold_rush_text_vectorDrawingImage",
            isPhoto = true,
            color_bg_primary = new SolidColorBrush { Color = Colors.Black },
            color_bg_secondary = App.Current.Resources["GoldRush_Gold"] as SolidColorBrush,
            color_text_primary = new SolidColorBrush { Color = Colors.White },
            color_text_secondary = new SolidColorBrush { Color = Colors.Black },
        };

        Design Heidner_Properties = new Design
        {
            canonical_title = "Heidner Properties",
            logoref = "heidner_vectorDrawingImage",
            isPhoto = true,
            color_bg_primary = new SolidColorBrush { Color = Colors.White },
            color_bg_secondary = App.Current.Resources["Heidner_Blue"] as SolidColorBrush,
            color_text_primary = new SolidColorBrush { Color = Colors.Black },
            color_text_secondary = App.Current.Resources["Heidner_Gray"] as SolidColorBrush,
        };

        /// <summary>
        /// Constructor for <code>CardPreview</code>
        /// </summary>
        public CardPreview()
        {
            InitializeComponent();

            Visitor_Photo.footer = footerdatesyncer();
            Gold_Rush.footer = goldrushgrid();

            Designs.Add(Visitor_Photo);
            Designs.Add(Visitor_Gold_Rush);
            Designs.Add(Visitor_Heidner_Properties);
            Designs.Add(Gold_Rush);
            Designs.Add(Heidner_Properties);
        }

        private UniformGrid footerdatesyncer()
        {
            Label label = new Label
            {
                Content = "Print Date",
                FontSize = 15.5,
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                //ContentStringFormat = "Admitted {0:T}",
            };

            System.Windows.Controls.Primitives.UniformGrid grid = new System.Windows.Controls.Primitives.UniformGrid
            {
                Columns = 1,
                Rows = 1
            };

            grid.Children.Add(label);

            return grid;
        }

        private UniformGrid goldrushgrid()
        {
            TextBlock goldrush_diamond = new TextBlock
            {
                Text = "♦",
                Margin = new Thickness(5),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock goldrush_heart = new TextBlock
            {
                Text = "♥",
                Margin = new Thickness(5),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock goldrush_spade = new TextBlock
            {
                Text = "♠",
                Margin = new Thickness(5),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock goldrush_club = new TextBlock
            {
                Text = "♣",
                Margin = new Thickness(5),
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            System.Windows.Controls.Primitives.UniformGrid goldrush_symbols = new System.Windows.Controls.Primitives.UniformGrid
            {
                Columns = 4,
                Rows = 1
            };

            goldrush_symbols.Children.Add(goldrush_diamond);
            goldrush_symbols.Children.Add(goldrush_heart);
            goldrush_symbols.Children.Add(goldrush_spade);
            goldrush_symbols.Children.Add(goldrush_club);

            return goldrush_symbols;
        }

        /// <summary>
        /// Change design of card
        /// </summary>
        public void SetDesign(object sender, RoutedEventArgs e)
        {
            /*
            switch (design) {
                case Designs.Gold_Rush:
                    setDesign(design: Gold_Rush);
                    break;
                case Designs.Heidner_Properties:
                    setDesign(design: Heidner_Properties);
                    break;
                default:
                    break;
            }
            */
            Design design = (sender as RadioButtonDesign).Design;

            setDesign(design);
        }
        private void setDesign(Design design)
        {
            //LogoImage.Source = design.logo;
            LogoImage.SetResourceReference(Image.SourceProperty, design.logoref);

            // Background colors
            CardDesign.Background = design.color_bg_primary;
            Sec2Color.Background = design.color_bg_secondary;

            if (design.isPhoto)
            {
                LogoImage2.Visibility = Visibility.Hidden;

                // Text colors
                NameFirst.Visibility = Visibility.Visible;
                NameFirst.Foreground = design.color_text_primary;
                NameLast.Visibility = Visibility.Visible;
                NameLast.Foreground = design.color_text_secondary;
                JobDescription.Visibility = Visibility.Visible;
                JobDescription.Foreground = design.color_text_secondary;

                NameFirst.Height = Double.NaN;
            }
            else
            {
                LogoImage2.Visibility = Visibility.Visible;

                NameFirst.Visibility = Visibility.Hidden;
                NameLast.Visibility = Visibility.Hidden;
                JobDescription.Visibility = Visibility.Hidden;

                NameFirst.Height = 0;

                LogoImage2.SetResourceReference(Image.SourceProperty, design.logo2ref);

                Bio person = App.Current.Resources["person"] as Bio;
                Uri unknownPerson = new Uri(@"pack://application:,,,/Resources/img/unkown person.png");
                person.Photo = new BitmapImage(unknownPerson);
            }

            // Clear footer and add new one if present
            Footer.Children.Clear();
            if (design.footer != null)
            {
                Footer.Children.Add(design.footer);
            }
        }

        /// <summary>
        /// Data for the design of a card
        /// </summary>
        public struct Design
        {
            public string canonical_title;
            public string logoref;
            public bool isPhoto;
            public string logo2ref;
            public Brush color_bg_primary;
            public Brush color_bg_secondary;
            public Brush color_text_primary;
            public Brush color_text_secondary;
            public Panel footer;
            public enum DesignType
            {
                Simple,
                Complex
            }
        }
    }
}
