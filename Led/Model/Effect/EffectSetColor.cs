using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Led.Model.Effect
{
    [JsonObject]
    public class EffectSetColor : EffectBase
    {
        public EffectSetColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.SetColor, startFrame, endFrame)
        {
            Colors = new List<Color>();
            Colors.Add(System.Windows.Media.Colors.Black);
        }

        public override List<LedChangeData> LedChangeDatas (long frame)
        {            
            if (frame == StartFrame)
            {
                List<LedChangeData> res = new List<LedChangeData>();
                Leds.ForEach(x => res.Add(new LedChangeData(x, Colors[0], 0)));
                return res;
            }

            return null;
        }
    }
}
