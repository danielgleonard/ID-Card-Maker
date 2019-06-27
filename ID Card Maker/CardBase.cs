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
    class CardBase : UserControl
    {
        public DateTime CurrentDateAndTime { get; set; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor for <code>CardPreview</code>
        /// </summary>
        public void CardPreview()
        {
        }

        /// <summary>
        /// Create a message regarding the current date and time
        /// </summary>
        /// <returns><code>UniformGrid</code> containing the text "Admitted" followed the current date and time</returns>
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
    }
}
