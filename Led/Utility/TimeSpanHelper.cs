using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    public static class TimeSpanHelper
    {
        private const string _TimeSpanDisplayFormat = @"mm\:ss\.fff";

        public static string ToDisplayString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(_TimeSpanDisplayFormat);
        }

        public static TimeSpan FromDisplayString(string displayString)
        {
            if (TimeSpan.TryParseExact(displayString, _TimeSpanDisplayFormat, CultureInfo.CurrentCulture, out TimeSpan result)
                || TimeSpan.TryParseExact(displayString, @"mm\:ss", CultureInfo.CurrentCulture, out result)
                || TimeSpan.TryParseExact(displayString, @"m\:ss", CultureInfo.CurrentCulture, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException("invalid formatted string: " + displayString);
            }

        }
    }
}
