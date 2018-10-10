using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Led.Model.Effect
{
    public abstract class EffectBase : IEffectLogic
    {
        public bool Active { get; set; }

        public EffectType EffectType { get; private set; }

        public ushort StartFrame { get; set; }

        public ushort Dauer => (ushort)(EndFrame > StartFrame ? EndFrame - StartFrame : 0);

        public ushort EndFrame { get; set; }

        public List<Utility.LedModelID> Leds { get; set; }

        public List<Point> LedPositions { get; set; }

        public short PositionPriority { get; set; }

        public short ColorPriority { get; set; }

        public ushort ID { get; set; }

        public EffectBase(EffectType effectType, ushort startFrame = 0, ushort endFrame = 0)
        {
            Active = true;
            EffectType = effectType;
            StartFrame = startFrame;
            EndFrame = endFrame;

            PositionPriority = 0;
            ColorPriority = 0;

            Leds = new List<Utility.LedModelID>();

            //Change later
            ID = 0;
        }

        public virtual List<LedChangeData> LedChangeDatas
        {
            get => throw new NotImplementedException();
        }
    }
}
