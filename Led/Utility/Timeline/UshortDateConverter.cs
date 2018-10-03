using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Led.Utility.Timeline
{
    class UshortDateConverter : IValueConverter
    {
		// TODO adds an offset of +/- 1 minute so the Date isn't exactly 00:00 but the long still is 0
		// 0 -> 00:01, 00:01 -> 0

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ushort l = (ushort)value;
            DateTime dt = DateTime.MinValue.AddMinutes(1 + l);
            //Console.WriteLine("Convert " + l + " -> " + dt);
            return dt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            ushort l = (ushort)(new TimeSpan(dt.Ticks).Subtract(new TimeSpan(0)).TotalMinutes - 1);
            //Console.WriteLine("ConvertBack " + dt + " -> " + l);
            return l;
        }
    }
}
