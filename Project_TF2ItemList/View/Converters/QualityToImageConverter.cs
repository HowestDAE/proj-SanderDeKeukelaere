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
    internal class QualityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string qualityName = "Unique";

            switch((int)value)
            {
                case 0:
                    qualityName = "Normal";
                    break;
                case 1:
                    qualityName = "Genuine";
                    break;
                case 3:
                    qualityName = "Vintage";
                    break;
                case 5:
                    qualityName = "Unusual";
                    break;
                case 7:
                    qualityName = "Community";
                    break;
                case 8:
                    qualityName = "Valve";
                    break;
                case 9:
                    qualityName = "Self-Made";
                    break;
                case 11:
                    qualityName = "Strange";
                    break;
                case 13:
                    qualityName = "Haunted";
                    break;
                case 14:
                    qualityName = "Collector's";
                    break;
                case 15:
                    qualityName = "Decorated";
                    break;
            }

            return new BitmapImage(new Uri($"pack://Application:,,,/Resources/Qualities/{qualityName}.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
