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

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if ((obj as LedModelID).BusID == BusID && (obj as LedModelID).PositionInBus == PositionInBus && (obj as LedModelID).Led == Led)
                return true;
            else
                return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int res = BusID;
            res = res << 8;
            res += PositionInBus;
            res = res << 8;
            res += Led;

            return res.GetHashCode();
        }
    }
}
