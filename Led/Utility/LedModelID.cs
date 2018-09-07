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
        public byte BusID;
        public byte PositionInBus;
        public ushort Led;

        public LedModelID(byte BusID, byte PositionInBus, ushort Led)
        {
            this.BusID = BusID;
            this.PositionInBus = PositionInBus;
            this.Led = Led;
        }
    }
}
