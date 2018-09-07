using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Led.Model.Effect
{
    public abstract class EffectBase
    {
        public bool Active;

        public EffectType EffectType { get; private set; }

        public short StartFrame { get; set; }

        public short EndFrame { get; set; }

        public List<Utility.LedModelID> Leds { get; set; }

        public List<Point> LedPositions { get; set; }

        public short PosPriority { get; set; }

        public short ColPriority { get; set; }

        public EffectBase(EffectType EffectType)
        {
            Active = true;
            this.EffectType = EffectType;
            StartFrame = 0;
            EndFrame = 1;

            PosPriority = 0;
            ColPriority = 0;

            Leds = new List<Utility.LedModelID>();
        }
    }
}
