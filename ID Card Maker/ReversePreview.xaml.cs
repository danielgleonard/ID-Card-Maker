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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ID_Card_Maker
{
    /// <summary>
    /// Interaction logic for ReversePreview.xaml
    /// </summary>
    public partial class ReversePreview : UserControl
    {
        public ReversePreview()
        {
            InitializeComponent();
        }
    }

    public class ListIndexConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert binding of <code>Strings</code> and <code>int</code> to index of <code>Strings</code>
        /// </summary>
        /// <param name="values">Array length 2 of <code>Strings</code> and <code>int</code></param>
        /// <param name="targetType">Should be <code>string</code></param>
        /// <returns><code>string</code> representing index <code>values[1]</code> of <code>values[0]</code></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Strings list = values[0] as Strings;
            int index    = (int)values[1];

            if(index > list.Count || index < -1)
            {
                throw new IndexOutOfRangeException(String.Format("Requested index {0} of Strings {1} does not exist", arg0:index, arg1:list));
            }
            else if (index == -1)
            {
                return null;
            }

            return list[index];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
