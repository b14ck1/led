using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Led.Utility
{
    public class LedModelID
    {
        public byte BusID { get; set; }
        public byte PositionInBus { get; set; }
        public ushort Led { get; set; }

        public LedModelID(byte busID, byte positionInBus, ushort led)
        {
            BusID = busID;
            PositionInBus = positionInBus;
            Led = led;
        }
    }
}
