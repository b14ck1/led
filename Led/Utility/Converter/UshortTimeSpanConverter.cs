using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Led.Utility.Converter
{
    class UshortTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort input = (ushort)value;
            return TimeSpan.FromMilliseconds(input * 1000 / Defines.FramesPerSecond);            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan input = (TimeSpan)value;
            return (ushort)(input.TotalMilliseconds * Defines.FramesPerSecond / 1000);            
        }
    }
}