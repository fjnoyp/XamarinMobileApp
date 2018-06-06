using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Converters
{
    public class LongToDateConverter : IValueConverter
    {
        public LongToDateConverter() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long ticks = (long)value;
            return (new DateTime(ticks)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
