using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Cards.Core.Converters
{
    public class BoolToFavIconConverter : IValueConverter
    {
        public BoolToFavIconConverter() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isFavorite = (bool)value;

            if (isFavorite)
            {
                return ImageSource.FromFile("FavoriteIcon.png"); 
            }
            else
            {
                return ImageSource.FromFile("NonFavoriteIcon.png"); 
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
