using System.Collections.Generic;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Led.Model.Effect
{
    [JsonObject]
    public class EffectBlinkColor : EffectBase
    {
        [JsonProperty]
        public ushort BlinkFrames { get; set; }

        public EffectBlinkColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Blink, startFrame, endFrame)
        {
            Colors = new List<Color>()
            {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.Black
            };
            BlinkFrames = 0;
        }

        public override List<LedChangeData> LedChangeDatas(long frame)
        {
            List<LedChangeData> res = new List<LedChangeData>();

            long currFrame = frame - StartFrame;
            if (currFrame >= 0 && currFrame <= EndFrame)
            { 
                if (currFrame % BlinkFrames == 0)
                {
                    int tmp = (int)(currFrame / BlinkFrames);
                    Leds.ForEach(x => res.Add(new LedChangeData(x, Colors[tmp % Colors.Count], 0)));                    
                }

                return res;
            }

            return null;
        }
    }
}
