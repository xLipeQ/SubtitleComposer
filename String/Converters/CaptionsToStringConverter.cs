using String.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace String.Converters
{
    public class CaptionsToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Subtitle> Captions = (ObservableCollection<Subtitle>)value;

            var caption = new StringBuilder(Captions.Count * 20);
            foreach(Subtitle film in Captions)
            {
                caption.AppendLine(film.Text);
            }
            return caption.ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
