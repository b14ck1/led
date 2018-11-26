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
            BlinkFrames = 1;
        }

        public override List<LedChangeData> LedChangeDatas(long frame)
        {
            bool _initialized = false;
            List<LedChangeData> res = null;

            long currFrame = frame - StartFrame;
            if (currFrame >= 0 && currFrame <= EndFrame)
            { 
                if (currFrame % BlinkFrames == 0)
                {
                    if (!_initialized)
                    {
                        res = new List<LedChangeData>();
                        _initialized = true;
                    }

                    int tmp = (int)(currFrame / BlinkFrames);
                    Leds.ForEach(x => res.Add(new LedChangeData(x, Colors[tmp % Colors.Count], 0)));                    
                }

                if(_initialized)
                    return res;
            }

            return null;
        }
    }
}
