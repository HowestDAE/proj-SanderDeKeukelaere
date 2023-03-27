using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Project_TF2ItemList.View.Converters
{
    public class QualityToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((int)value)
            {
                case 0:
                    return "Normal";;
                case 1:
                    return "Genuine";
                case 3:
                    return "Vintage";
                case 5:
                    return "Unusual";
                case 7:
                    return "Community";
                case 8:
                    return "Valve";
                case 9:
                    return "Self-Made";
                case 11:
                    return "Strange";
                case 13:
                    return "Haunted";
                case 14:
                    return "Collector's";
                case 15:
                    return "Decorated";
                default:
                    return "Unique";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
