using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    public class EffectBlinkColor : EffectBase
    {
        public List<Color> Colors { get; set; }
        public ushort BlinkFrames { get; set; }
        public ushort CurrentColor { get; set; }

        public EffectBlinkColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Blink, startFrame, endFrame)
        {
            Colors = new List<Color>()
            {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.Black
            };
            BlinkFrames = BlinkFrames;
            CurrentColor = 0;
        }
    }
}
