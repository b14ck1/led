using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    public class EffectBlinkColor : EffectBase
    {
        public List<Color> Colors { get; set; }
        public ushort BlinkFrames { get; set; }
        private ushort CurrentColor { get; set; }

        public EffectBlinkColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Blink, startFrame, endFrame)
        {
            Colors = new List<Color>()
            {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.Black
            };
            BlinkFrames = 0;
            CurrentColor = 0;
        }

        public override List<LedChangeData> LedChangeDatas
        {
            get
            {
                List<LedChangeData> res = new List<LedChangeData>();

                for (int i = 0; i < Dauer; i++)
                {
                    if (i % BlinkFrames == 0)
                    {
                        res.Add(new LedChangeData(Leds, Colors[CurrentColor], 0));
                        CurrentColor++;

                        if (CurrentColor == Colors.Count)
                            CurrentColor = 0;
                    }
                }

                return res;
            }
        }
    }
}
