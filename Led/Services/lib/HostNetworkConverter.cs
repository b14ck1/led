using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services.lib
{
    public static class HostNetworkConverter
    {
        static byte[] _Int16 = new byte[2];
        static byte[] _Int32 = new byte[2];

        public static Int16 Int16(Int16 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                _Int16 = BitConverter.GetBytes(value);
                return (BitConverter.ToInt16(_Int16.Reverse().ToArray(), 0));
            }
            else
                return value;
        }
        
        public static Int32 Int32(Int32 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                _Int32 = BitConverter.GetBytes(value);
                return (BitConverter.ToInt32(_Int32.Reverse().ToArray(), 0));
            }
            else
                return value;
        }
    }
}
