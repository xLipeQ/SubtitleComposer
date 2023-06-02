using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace String.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = (TimeSpan)value;

            string _timeasstr = new DateTime(time.Ticks).ToString("HH:mm:ss.fff");

            bool was_dot = false;
            while (_timeasstr[_timeasstr.Length - 1] == '0' || (was_dot = _timeasstr[_timeasstr.Length - 1] == '.'))
            {
                _timeasstr = _timeasstr.Substring(0, _timeasstr.Length - 1);
                if (was_dot)
                    break;
            }

            while (_timeasstr.Length > 1 && (_timeasstr[0] == '0' || _timeasstr[0] == ':'))
            {
                if (_timeasstr.Length >= 2 && _timeasstr[1] == '.')
                    break;
                _timeasstr = _timeasstr.Substring(1);
            }

            return _timeasstr;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            var time = str.Split('.');

            int milisec = 0;

            if(time.Length > 1 && time[1].Length > 0)
            {

                while (time[1].Length != 3)                
                    time[1] += '0';
                
                if (!int.TryParse(time[1], out milisec))
                    return "a";
                while (time[1].Length > 0 && time[1][0] == '0')
                {
                    milisec *= 10;
                    time[1] = time[1].Substring(1);
                }

            }

            var time2 = time[0].Split(':');

            // seconds, minutes, hours
            int[] time_span = new int[3];

            int i = 0;
            foreach(var t in time2.Reverse())
            {
                if (t.Length > 2)
                    return "a";
                if (!int.TryParse(t, out time_span[i]))
                    return "a";
                i++;
            }

            return TimeSpan.Parse($"{time_span[2]}:{time_span[1]}:{time_span[0]}") + TimeSpan.FromMilliseconds(milisec);

        }
    }
}