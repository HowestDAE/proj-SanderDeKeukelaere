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
    public class QualityToHexColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((int)value)
            {
                case 0:
                    return "#B2B2B2";
                case 1:
                    return "#4D7455";
                case 3:
                    return "#476291";
                case 5:
                    return "#8650AC";
                case 7:
                    return "#70B04A";
                case 8:
                    return "#A50F79";
                case 9:
                    return "#70B04A";
                case 11:
                    return "#CF6A32";
                case 13:
                    return "#38F3AB";
                case 14:
                    return "#AA00000";
                case 15:
                    return "#FAFAFA";
                default:
                    return "#FFD700";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
