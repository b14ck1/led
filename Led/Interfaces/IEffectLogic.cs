using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led
{
    public interface IEffectLogic
    {
        void Calc(IEffectLogic effectLogic);

        ushort StartFrame { get; }

        ushort Dauer { get; }

        ushort EndFrame { get; }

        List<Utility.LedModelID> Leds { get; }
        
        short PositionPriority { get; }

        short ColorPriority { get; }
    }
}
