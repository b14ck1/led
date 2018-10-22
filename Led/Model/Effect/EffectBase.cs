using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Led.Model.Effect
{
    [JsonObject]
    public abstract class EffectBase : IEffectLogic
    {
        [JsonProperty]
        public bool Active { get; set; }

        [JsonProperty]
        public EffectType EffectType { get; private set; }

        [JsonProperty]
        public ushort StartFrame { get; set; }
        
        public ushort Dauer => (ushort)(EndFrame > StartFrame ? EndFrame - StartFrame : 0);

        [JsonProperty]
        public ushort EndFrame { get; set; }

        [JsonProperty]
        public List<System.Windows.Media.Color> Colors { get; set; }

        [JsonProperty]
        public List<Utility.LedModelID> Leds { get; set; }

        [JsonProperty]
        public List<Point> LedPositions { get; set; }

        [JsonProperty]
        public short PositionPriority { get; set; }

        [JsonProperty]
        public short ColorPriority { get; set; }

        [JsonProperty]
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

        public virtual List<LedChangeData> LedChangeDatas (long frame)
        {
            throw new NotImplementedException();
        }
    }
}
