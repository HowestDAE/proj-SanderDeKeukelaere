﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Project_TF2ItemList.View.Converters
{
    public class QualityToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string qualityName = new QualityToNameConverter().Convert(value, targetType, parameter, culture).ToString();

            return new BitmapImage(new Uri($"pack://Application:,,,/Resources/Qualities/{qualityName}.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
